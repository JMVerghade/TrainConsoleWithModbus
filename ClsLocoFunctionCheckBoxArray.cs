using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Cs_Modbus_Maitre
{
    class LocoFunctionCheckBoxArray : System.Collections.CollectionBase
    {
        private readonly System.Windows.Forms.Form HostForm;
        public void AddNewCheckBox()
        {
            // Crée une nouvelle instance de la classe Button.
            System.Windows.Forms.CheckBox aCheckBox = new System.Windows.Forms.CheckBox();
            // Ajoute le bouton à la liste interne de la collection.
            this.List.Add(aCheckBox);
            // Ajoute le bouton à la collection de contrôles du formulaire 
            // référencé dans le champ HostForm.
            HostForm.Controls.Add(aCheckBox);
            // Définit les propriétés initiales de l'objet button.
            aCheckBox.Top = 250 + (Count-1) % 4 * 25;
            aCheckBox.Left = 10+(Count-1) / 4 *100;
            aCheckBox.Width = 100;
            aCheckBox.Tag = this.Count;
            aCheckBox.Text = "F " + this.Count.ToString();
        }

        public LocoFunctionCheckBoxArray(System.Windows.Forms.Form host)
        {
         HostForm = host;
         this.AddNewCheckBox();
        }

        public System.Windows.Forms.CheckBox this[int Index]
        {
            get
            {
                return (System.Windows.Forms.CheckBox)this.List[Index];
            }
        }
        public void Remove()
        {
         // Vérifie qu'il existe un bouton à supprimer.
         if (this.Count > 0)
         {
         // Supprime le dernier bouton ajouté 
         // au tableau de la collection des contrôles 
         // du formulaire hôte. Remarquez l'utilisation 
         // de l'indexeur en accédant au tableau.
         HostForm.Controls.Remove(this[this.Count -1]);
         this.List.RemoveAt(this.Count -1);
         }

        }

    }
}
