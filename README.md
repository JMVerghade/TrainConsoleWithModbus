TrainConsoleWithModbus
======================
This program is developped with Microsoft Visual Studio Express 2012
with Visual C# on Windows 7

//--------------------------------------------------------------
// JMV 2014 : Train Console with Modbus communiation
//--------------------------------------------------------------
// This programm is a demonstration program to confirm that
// controling DCC Locomotives with an Arduino UNO or MEGA 
// connected to a PC through the USB port - RS232 and with Modbus 
// RTU protocol is possible.
// This programm is the Modbus master and polls Arduino to get
// informations. When the user changes the throttle of a locomotive
// a modbus frame with special function code and datas is send to
// the Arduino. The Arduino answers to the Modbus master and 
// executes the DCC command
//--------------------------------------------------------------
// I started this code thanks to the following source code because 
// it was very easy to use, and Modbus TCP sounds nice to me for
// further development of this train console with Modbus
//--------------------------------------------------------------
//  Client Modbus Maitre     B.Vannier www.va2i.com 31 juil 2008
//--------------------------------------------------------------
// Application test MODBUS
// Module associé: GModbus_seul_2008.cs 
//                 TMODBUS_S: gestion des protocoles MODBUS et MODBUS TCP
//--------------------------------------------------------------
// Ce logiciel est totalement libre de droits
//  Néanmoins, merci de m'envoyer un email en cas d'utilisation
//  avec éventuellement vos remarques à bruno.vannier@va2i.com
//--------------------------------------------------------------
