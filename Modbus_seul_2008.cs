//---------------------------------------------------------------------------
// This code is written by B Vannier.
// I modified the code to add new Modbus function codes to send DCC Train 
// commands.
// I defined only three extended functions
// trame_writeLoco : Fonction 50 
// trame_writeLocoFunction : Fonction 51 
// trame_writeEmergencyStop : Fonction 52 
// JMV- 2014
//---------------------------------------------------------------------------
// MODBUS_SEUL_2008.cs                   B.Vannier www.va2i.com juil 2008
//  Classe Modbus_S et TTCP_MODBUS_S
//  Gestion des protocoles MODBUS et MODBUS TCP
//---------------------------------------------------------------------------
// Ce logiciel est totalement libre de droits
//  Néanmoins, merci de m'envoyer un email en cas d'utilisation
//  avec éventuellement vos remarques à bruno.vannier@va2i.com
//--------------------------------------------------------------
// Modifs:
//------------------------------------------------------------------------
// Classe permettant de construire et d'interpreter des trames modbus
// Les accès au port ne sont pas gérés dans cet objet
// les fonctions suivantes sont implémentées
//      Lecture bits  : code 1
//      Lecture bits  : code 2
//      Lecture mots  : code 3
//      Lecture mots  : code 4
//      Ecriture bits : code 15
//      Ecriture mots : code 16
//
// construction de la trame lecture bits:
//   trame_readbit();
// construction de la trame lecture mots:
//   trame_readT();
// analyse de la trame retour lecture mots:
//   extrait_readT();
// construction de la trame écriture bits:
//   function trame_writebit();
// construction de la trame écriture mots:
//   function trame_writeT();
//------------------------------------------------------------------------
// A terminer:
//   analyse trame retour fonctions 15 et 16: écriture bits et mots
//------------------------------------------------------------------------

//---------------------------------------------------------------------------
namespace Cs_Modbus_Maitre
{
    public partial class TMODBUS_S
    {
        const byte LOCO=0x50;
        const byte  LOCO_FUNCTION=0x51;
        const byte  LOCO_ESD=0x52;
        //---------------------------------------------------------------------------
        protected ushort CRC_16(ref byte[] tib, int n_oct)
        {
            uint n, retenue = 0, bit;
            uint crc = 0xffff;

            for (n = 0; n < n_oct; n++)
            {
                crc = (crc ^ tib[n]);         // XOR entre crc et l'octet 
                for (bit = 0; bit < 8; bit++)
                {
                    retenue = (crc & 0x1);   // retenue du decalage suivant 
                    crc = (crc >> 1);       // DECALAGE A DROITE DE 1 BIT  
                    crc = crc & 0x7fff;
                    if (retenue == 1)
                    {
                        crc = (crc ^ (uint)0xA001);   // XOR entre crc et polynome  
                    }
                }
            }
            return ((ushort)crc);
        }
        //---------------------------------------------------------------------------
        // ajout du CRC 
        protected int ajout_crc(ref byte[] tib, int n_oct)
        {
            ushort crc = CRC_16(ref tib, n_oct);
            tib[n_oct++] = (byte)(crc & 0xFF);
            tib[n_oct++] = (byte)((crc & 0xFF00) >> 8);
            return (n_oct);

        }
        //---------------------------------------------------------------------------
        protected ushort mot(byte fort, byte faible)
        {
            ushort us_fort = (ushort)fort;
            us_fort <<= 8;
            ushort us_faible = (ushort)faible;
            return ((ushort)(us_fort | us_faible));
        }
        //----------------------------------------------------------------------------
        // trame_readBit : Fonction 1 modbus, construction trame lecture de bits
        //----------------------------------------------------------------------------
        public int trame_readbit1(int adr, int nbit, int nes, ref byte[] Tabemi)
        {

            Tabemi[0] = (byte)(nes & 0xFF);       // No d'esclave
            Tabemi[1] = 1;                // No de fonction
            Tabemi[2] = (byte)((adr >> 8) & 0xFF);  // adresse
            Tabemi[3] = (byte)(adr & 0xFF);
            Tabemi[4] = (byte)((nbit >> 8) & 0xFF);// nb de bits
            Tabemi[5] = (byte)(nbit & 0xFF);
            return (ajout_crc(ref Tabemi, 6));        // nb d'octets de la trame
        }
        //----------------------------------------------------------------------------
        // trame_readBit : Fonction 2 modbus, construction trame lecture de bits
        //----------------------------------------------------------------------------
        public int trame_readbit2(int adr, int nbit, int nes, ref byte[] Tabemi)
        {

            Tabemi[0] = (byte)(nes & 0xFF);       // No d'esclave
            Tabemi[1] = 2;                // No de fonction
            Tabemi[2] = (byte)((adr >> 8) & 0xFF);  // adresse
            Tabemi[3] = (byte)(adr & 0xFF);
            Tabemi[4] = (byte)((nbit >> 8) & 0xFF);// nb de bits
            Tabemi[5] = (byte)(nbit & 0xFF);
            return (ajout_crc(ref Tabemi, 6));        // nb d'octets de la trame
        }
        //----------------------------------------------------------------------------
        // extrait_readbit : Fonction 2 modbus, analyse de la trame retour
        //----------------------------------------------------------------------------
        public int extrait_readbit(ref bool[] valeur, ref byte[] Tabrec)
        {
            int nbit, index_crc, i, j, requete;
            byte octet;
            ushort crc;

            requete = Tabrec[1];
            if ((requete == 1) || (requete == 2))
            {
                nbit = (Tabrec[2]) << 3;    // nbits = nb d'octets x 8
                for (i = 0; i < Tabrec[2]; i++)
                {   // octet par octet
                    octet = (byte)Tabrec[i + 3];
                    for (j = 0; j < 8; j++)
                    {             // puis bit par bit
                        if (((octet >> j) & 1) != 0) valeur[(i << 3) + j] = true;
                        else valeur[(i << 3) + j] = false;
                    }
                }
                //-------- Test du CRC
                index_crc = Tabrec[2] + 3;
                crc = CRC_16(ref Tabrec, index_crc);
                if ((Tabrec[index_crc] == (byte)(crc & 0xFF))
                    && (Tabrec[index_crc + 1] == (byte)((crc >> 8) & 0xFF)))
                    return (nbit);
                else return (-4);
            }
            else return (-3);
        }
        //---------------------------------------------------------------------------
        // trame_readT : Fonction 3 modbus, construction trame lecture de plusieurs mots
        //----------------------------------------------------------------------------
        public int trame_readT3(int adr, int nbmot, int nes, ref byte[] Tabemi)
        {
            Tabemi[0] = (byte)(nes & 0xFF);       // No d'esclave
            Tabemi[1] = 3;                // No de fonction
            Tabemi[2] = (byte)((adr >> 8) & 0xFF);  // adresse
            Tabemi[3] = (byte)(adr & 0xFF);
            Tabemi[4] = (byte)((nbmot >> 8) & 0xFF);// nb de mots
            Tabemi[5] = (byte)(nbmot & 0xFF);
            return (ajout_crc(ref Tabemi, 6));               // retourne le nb d'octets de la trame
        }
        //---------------------------------------------------------------------------
        // trame_readT : Fonction 4 modbus, construction trame lecture de plusieurs mots
        //----------------------------------------------------------------------------
        public int trame_readT4(int adr, int nbmot, int nes, ref byte[] Tabemi)
        {
            Tabemi[0] = (byte)(nes & 0xFF);       // No d'esclave
            Tabemi[1] = 4;                // No de fonction
            Tabemi[2] = (byte)((adr >> 8) & 0xFF);  // adresse
            Tabemi[3] = (byte)(adr & 0xFF);
            Tabemi[4] = (byte)((nbmot >> 8) & 0xFF);// nb de mots
            Tabemi[5] = (byte)(nbmot & 0xFF);
            return (ajout_crc(ref Tabemi, 6));               // retourne le nb d'octets de la trame
        }
        //---------------------------------------------------------------------------
        // extrait_readT : Fonction 3 ou 4 modbus, analyse de la trame retour
        //----------------------------------------------------------------------------
        public int extrait_readT(ref ushort[] valeur, ref byte[] Tabrec)
        {
            int nbmot, index_crc, i, requete;
            ushort crc;

            requete = Tabrec[1];
            if ((requete == 4) || (requete == 3))
            {
                nbmot = Tabrec[2] >> 1;    // nombre de mots lus = nb octets / 2
                for (i = 0; i < nbmot; i++)
                {
                    valeur[i] = mot(Tabrec[(i << 1) + 3], Tabrec[(i << 1) + 4]);
                    //        valeur[i] = (Tabrec[(i<<1)+4]) | ((short)(Tabrec[(i<<1)+3]) << 8);
                }
                //-------- Test du CRC
                index_crc = Tabrec[2] + 3;
                crc = CRC_16(ref Tabrec, index_crc);
                if ((Tabrec[index_crc] == (byte)(crc & 0xFF))
                    && (Tabrec[index_crc + 1] == (byte)((crc >> 8) & 0xFF)))
                    return (nbmot);
                else return (-4);
            }
            else return (-3);
        }
        //----------------------------------------------------------------------------
        // trame_writebit : Fonction 15 modbus, construction trame écriture de plusieurs bits
        //----------------------------------------------------------------------------


        public int trame_writebit(int adr, int nbit, int nes, ref bool[] valeurb, ref byte[] Tabemi)
        {
            int i, j, nboctet;
            byte octet;

            Tabemi[0] = (byte)(nes & 0xFF);       // No d'esclave
            Tabemi[1] = 15;                // No de fonction 15 écriture bits
            Tabemi[2] = (byte)((adr >> 8) & 0xFF);  // adresse
            Tabemi[3] = (byte)(adr & 0xFF);
            Tabemi[4] = (byte)((nbit >> 8) & 0xFF);// nb de mots
            Tabemi[5] = (byte)(nbit & 0xFF);
            nboctet = (nbit + 7) >> 3;         // nb d'octets utiles
            Tabemi[6] = (byte)nboctet;        // nb d'octets


            for (i = 0; i < nboctet; i++)
            {   // octet par octet
                octet = 0;
                for (j = 0; j < 8; j++)
                {        // puis bit par bit
                    if ((valeurb[(i << 3) + j]) == true) octet = (byte)(octet | (1 << j));
                }
                Tabemi[i + 7] = (byte)octet;
            }

            nboctet = 7 + nboctet;        // nb d'octets sans CRC
            return (ajout_crc(ref Tabemi, nboctet));    // retourne le nb d'octets de la trame avec CRC
        }
        //----------------------------------------------------------------------------
        // trame_writeT : Fonction 16 modbus, construction trame écriture de plusieurs mots
        //----------------------------------------------------------------------------
        public int trame_writeT(int adr, int nbmot, int nes, ref ushort[] valeur, ref byte[] Tabemi)
        {
            int i, nboctet;

            Tabemi[0] = (byte)(nes & 0xFF);       // No d'esclave
            Tabemi[1] = 16;                // No de fonction
            Tabemi[2] = (byte)((adr >> 8) & 0xFF);  // adresse
            Tabemi[3] = (byte)(adr & 0xFF);
            Tabemi[4] = (byte)((nbmot >> 8) & 0xFF);// nb de mots
            Tabemi[5] = (byte)(nbmot & 0xFF);
            Tabemi[6] = (byte)(nbmot << 1);       // nb d'octets

            for (i = 0; i < nbmot; i++)
            {
                Tabemi[7 + (i << 1)] = (byte)((valeur[i] >> 8) & 0xFF);
                Tabemi[8 + (i << 1)] = (byte)(valeur[i] & 0xFF);
            }
            nboctet = 7 + (nbmot << 1);
            return (ajout_crc(ref Tabemi, nboctet));    // retourne le nb d'octets de la trame

        }
        //----------------------------------------------------------------------------
        // trame_writeLoco : Fonction 50 modbus, construction trame écriture de plusieurs mots
        //    adresse esclave
        //    code fonction : 50 pour Loco  
        //    adresse loco : 1 à xx
        //    direction : 1 ou 0
        //    vitesse : 0 à 128
        //----------------------------------------------------------------------------
        public int trame_writeLoco(int LocoAdr, int LocoDirection, int LocoSpeed, int nes, ref byte[] Tabemi)
        {
            int nboctet;

            Tabemi[0] = (byte)(nes & 0xFF);       // No d'esclave
            Tabemi[1] = LOCO;                // No de fonction
            Tabemi[2] = (byte)((LocoAdr >> 8) & 0xFF);  // adresse loco
            Tabemi[3] = (byte)(LocoAdr & 0xFF);
            int nbmot = 2;
            Tabemi[4] = (byte)((nbmot >> 8) & 0xFF);// nb de mots dans la trame
            Tabemi[5] = (byte)(nbmot & 0xFF);
            Tabemi[6] = 0;
            Tabemi[7] = (byte)(LocoDirection);       // nb d'octets
            Tabemi[8] = 0;
            Tabemi[9] = (byte)(LocoSpeed); 
            nboctet = 10;
            return (ajout_crc(ref Tabemi, nboctet));    // retourne le nb d'octets de la trame
        }
        //----------------------------------------------------------------------------
        // trame_writeEmergencyStop : Fonction 52 modbus, construction trame écriture de plusieurs mots
        //    adresse esclave
        //    code fonction : 52 for ESD 
        //    adresse loco : 1 à xx
        //----------------------------------------------------------------------------
        public int trame_writeESD(int LocoAdr, int nes, ref byte[] Tabemi)
        {
            int nboctet;

            Tabemi[0] = (byte)(nes & 0xFF);       // No d'esclave
            Tabemi[1] = LOCO_ESD;                // No de fonction
            Tabemi[2] = (byte)((LocoAdr >> 8) & 0xFF);  // adresse loco
            Tabemi[3] = (byte)(LocoAdr & 0xFF);
            int nbmot = 0;
            Tabemi[4] = (byte)((nbmot >> 8) & 0xFF);// nb de mots dans la trame
            Tabemi[5] = (byte)(nbmot & 0xFF);
            nboctet = 6;
            return (ajout_crc(ref Tabemi, nboctet));    // retourne le nb d'octets de la trame
        }
        //----------------------------------------------------------------------------
        // trame_writeLocoFunction : Fonction 51 modbus, construction trame écriture de plusieurs mots
        //    adresse esclave
        //    fonction nb : 51 pour Loco  Function
        //    adresse loco : 1 à xx
        //    Function : 0 to 
        //    Valeur de la fonction : 0 or 1
        //----------------------------------------------------------------------------
        public int trame_writeLocoFunction(int LocoAdr, int LocoFunctionNb, bool LocoFunctionValue, int nes, ref byte[] Tabemi)
        {
            int nboctet;

            Tabemi[0] = (byte)(nes & 0xFF);       // No d'esclave
            Tabemi[1] = LOCO_FUNCTION;                // No de fonction
            Tabemi[2] = (byte)((LocoAdr >> 8) & 0xFF);  // adresse loco
            Tabemi[3] = (byte)(LocoAdr & 0xFF);
            int nbmot = 2;
            Tabemi[4] = (byte)((nbmot >> 8) & 0xFF);// nb de mots dans la trame
            Tabemi[5] = (byte)(nbmot & 0xFF);
            Tabemi[6] = 0;
            Tabemi[7] = (byte)(LocoFunctionNb);       // Loco function number
            Tabemi[8] = 0;
            if (LocoFunctionValue) { Tabemi[9] = (byte)1; } else { Tabemi[9] = (byte)0; } // loco function value
            
            nboctet = 10;
            return (ajout_crc(ref Tabemi, nboctet));    // retourne le nb d'octets de la trame
        }
    } // fin TMODBUS_S


//----------------------------------------------------------------------------
// Version Modbus TCP (sans CRC)
//----------------------------------------------------------------------------
// Modbus_to_Tcp : adapte la trame Modbus d'origine avec CRC à la trame
//  modbus TCP sans CRC
// Attention la trame d'origine doit commencer en buffer_[6] (no d'esclave
//   ou unit identifier dans la nouvelle terminologie)
//---------------------------------------------------------------------------
                    /* -------------------------

    public partial class TTCP_TMODBUS_S
    {

int TCP_Modbus_to_Tcp(ref byte[] buffer_,int nb1)
{
//    transactionId = random( 0xFFFF );  // One could use greater numbers here
    transacId_emi++;
    buffer_[0] = (byte) (transacId_emi >> 8);;   // transaction identifier - copied by server
    buffer_[1] = (byte) (transacId_emi & 0xFF);; // transaction identifier - copied by server
    buffer_[2] = 0;      // protocol identifier MODBUS = 0
    buffer_[3] = 0;      // protocol identifier MODBUS = 0
    buffer_[4] = 0;              // longueur trame (sans CRC) poids forts
    buffer_[5] = (nb1-2) & 0xFF; // longueur trame (sans CRC) poids faibles
    return (nb1+4); // longueur de la nouvelle trame sans CRC mais avec entete
}
//----------------------------------------------------------------------------
int TCP_trame_readbit1(int adr, int nbit, int nes, ref byte[] Tabemi)
{
    int nb = trame_readbit1(adr, nbit, nes, &Tabemi[6]);
    return (TCP_Modbus_to_Tcp(Tabemi, nb));
}
//----------------------------------------------------------------------------
int TCP_trame_readbit2(int adr, int nbit, int nes, ref byte[] Tabemi)
{
    int nb = trame_readbit2(adr, nbit, nes, &Tabemi[6]);
    return (TCP_Modbus_to_Tcp(Tabemi, nb));
}
//----------------------------------------------------------------------------
int TCP_trame_readT3(int adr, int nbmot, int nes, ref byte[] Tabemi)
{
    int nb = trame_readT3(adr, nbmot, nes, &Tabemi[6]);
    return ( TCP_Modbus_to_Tcp(Tabemi, nb));
}
//----------------------------------------------------------------------------
int TCP_trame_readT4(int adr, int nbmot, int nes, ref byte[] Tabemi)
{
    int nb = trame_readT4(adr, nbmot, nes, &Tabemi[6]);
    return ( TCP_Modbus_to_Tcp(Tabemi, nb));
}
//----------------------------------------------------------------------------
int TCP_trame_writebit(int adr, int nbit, int nes, ref bool[] valeurb, ref byte[] Tabemi)
{
    int nb = trame_writebit(adr, nbit, nes, valeurb, &Tabemi[6]);
    return ( TCP_Modbus_to_Tcp(Tabemi, nb));
}
//----------------------------------------------------------------------------
int TCP_trame_writeT(int adr, int nbmot, int nes, ref short[] valeur, ref byte[] Tabemi)
{
    int nb = trame_writeT(adr, nbmot, nes, valeur, &Tabemi[6]);
    return ( TCP_Modbus_to_Tcp(Tabemi, nb));
}
//---------------------------------------------------------------------------
// TCP_extrait_readT : Fonction 3 ou 4 modbus, analyse de la trame retour
// version Modbus TCP
//----------------------------------------------------------------------------
int TCP_extrait_readT(ref short[] valeur, ref byte[] Tabrec)
{
  int  nbmot, index_crc, i, requete;
  transacId_rec = extrait_noreq_rec(Tabrec);   // on stocke le transaction identifier reçu
  Tabrec += 6; // on recale le pointeur sur trame pour TCP Modbus
  requete = Tabrec[1];
  if ((requete == 4 ) || (requete == 3)) {
    nbmot = Tabrec[2] >> 1;    // nombre de mots lus = nb octets / 2
    for (i= 0; i<nbmot; i++) {
      valeur[i] = (Tabrec[(i<<1)+4]) | ((short)(Tabrec[(i<<1)+3]) << 8);
    }
    return(nbmot);
  }
  else return(-3);
}
//----------------------------------------------------------------------------
// extrait_readbit : Fonction 1 ou 2 modbus, analyse de la trame retour
// version Modbus TCP
//----------------------------------------------------------------------------
int TCP_extrait_readbit(ref bool[] valeur, ref byte[] Tabrec)
{
  int  nbit, index_crc, i, j, requete;
  byte octet;

  transacId_rec = extrait_noreq_rec(Tabrec);   // on stocke le transaction identifier reçu
  Tabrec += 6; // on recale le pointeur sur trame pour TCP Modbus
  requete = Tabrec[1];
  if ((requete == 1 ) || (requete == 2)) {
    nbit = (Tabrec[2]) << 3;    // nbits = nb d'octets x 8
    for (i=0; i<Tabrec[2]; i++){   // octet par octet
      octet = (Tabrec[i+3]) & 0xFF;
      for (j=0; j<8; j++) {             // puis bit par bit
        if (((octet >> j) & 1) != 0) valeur[(i << 3) + j] = true;
                                else valeur[(i << 3) + j] = false;
      }
    }
    return(nbit);
  }
  else return(-3);
}
            } // fin TTCP_TMODBUS_S       ---------------------*/


} // namespace
