﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SQLite;

namespace LibraryProject
{
    class Database
    {
        private static string conn = "data source=library.db";

        //Vrne seznam vseh uporabnikov v bazi
        public static List<Uporabniki> izberiVseUporabnike()
        {
            List<Uporabniki> seznam = new List<Uporabniki>();
            using (SQLiteConnection con = new SQLiteConnection(conn))
            {
                con.Open();
                SQLiteCommand com = new SQLiteCommand("SELECT id, ime, priimek FROM uporabniki", con);
                SQLiteDataReader reader = com.ExecuteReader();
                while (reader.Read())
                {
                    int id = reader.GetInt32(0);
                    string ime = reader.GetString(1);
                    string priimek = reader.GetString(2);
                    seznam.Add(new Uporabniki(id, ime, priimek));
                }
                con.Close();
            }
            return seznam;
        }

        //Vrne seznam vseh uporabnikov, ki v imenu ali priimku vsebujejo vnešene znake
        public static List<Uporabniki> IsciVseUporabnike(string filter)
        {
            List<Uporabniki> seznam = new List<Uporabniki>();
            using (SQLiteConnection con = new SQLiteConnection(conn))
            {
                con.Open();
                SQLiteCommand com = new SQLiteCommand("SELECT id, ime, priimek FROM uporabniki WHERE ime LIKE '%" + filter + "%' OR priimek LIKE '%" + filter + "%';", con);
                SQLiteDataReader reader = com.ExecuteReader();
                while (reader.Read())
                {
                    int id = reader.GetInt32(0);
                    string ime = reader.GetString(1);
                    string priimek = reader.GetString(2);
                    seznam.Add(new Uporabniki(id, ime, priimek));
                }
                con.Close();
            }
            return seznam;
        }

        //Vrne seznam izposojenega gradiva od uporabnika
        public static List<Knjiga> izpisIzposojenegaGradiva(int uporabnik_id)
        {
            List<Knjiga> seznam = new List<Knjiga>();
            using (SQLiteConnection con = new SQLiteConnection(conn))
            {
                con.Open();
                SQLiteCommand com = new SQLiteCommand("SELECT id, naslov, leto_izdaje FROM knjige k INNER JOIN izposoje i ON k.id = i.knjiga_id INNER JOIN uporabniki u ON i.uporabnik_id = u.id WHERE u.id = '" + uporabnik_id + "';", con);
                SQLiteDataReader reader = com.ExecuteReader();
                while (reader.Read())
                {
                    string ime = reader.GetString(0);
                    string priimek = reader.GetString(1);
                    int id = reader.GetInt32(0);
                    string naslov = reader.GetString(1);
                    string leto_izdaje = reader.GetString(2);
                    seznam.Add(new Knjiga(id, naslov, leto_izdaje));
                }
                con.Close();
            }
            return seznam;
        }

        //Doda novega člana
        public static void dodajClana(string ime, string priimek, string telefon, string naslov, string email, string opombe)
        {
            try
            {
                //Spremeni vse empty strings v null
                if (telefon.Trim() == "")
                {
                    telefon = null;
                }
                if (naslov.Trim() == "")
                {
                    naslov = null;
                }
                if (email.Trim() == "")
                {
                    email = null;
                }
                if (opombe.Trim() == "")
                {
                    opombe = null;
                }

                using (SQLiteConnection con = new SQLiteConnection(conn))
                {
                    con.Open();
                    SQLiteCommand com = new SQLiteCommand("INSERT INTO uporabniki (ime, priimek, telefon, naslov, email, opombe) VALUES ('" + ime + "', '" + priimek + "', '" + telefon + "', '" + naslov + "', '" + email + "', '" + opombe + "');", con);
                    com.ExecuteNonQuery();
                    con.Close();
                }

                System.Windows.Forms.MessageBox.Show("Uporabnik uspešno dodan!");
            }
            catch (Exception)
            {
                System.Windows.Forms.MessageBox.Show("Napaka pri dodajanju novega člana");
            }
        }

        //Avtentikacija podatkov za prijavo uporabnika
        public static bool Prijava(string uporabniskoIme, string geslo)
        {

            using (SQLiteConnection con = new SQLiteConnection(conn))
            {
                con.Open();
                SQLiteCommand com = new SQLiteCommand("SELECT * FROM admins WHERE(uporabniskoIme = '" + uporabniskoIme + "') AND (geslo = '" + geslo + "');", con);
                SQLiteDataReader reader = com.ExecuteReader();
                if (reader.HasRows)
                {
                    con.Close();
                    return true;
                }
                else
                {
                    con.Close();
                    return false;
                }
            }
        }

        public static void Registracija(string ime, string priimek, string uporabniskoIme, string geslo)
        {
            using (SQLiteConnection con = new SQLiteConnection(conn))
            {
                con.Open();
                SQLiteCommand com = new SQLiteCommand("INSERT INTO admins(ime, priimek, uporabniskoIme, geslo) " +
                    "VALUES('" + ime + "', '" + priimek + "', '" + uporabniskoIme + "', '" + geslo + "'); ", con);
                //SQLiteDataReader reader = com.ExecuteReader();
                com.ExecuteNonQuery();
            }
        }

        public static List<Gradivo> IzberiVsoGradivo()
        {
            List<Gradivo> seznamGradiva = new List<Gradivo>();

            using (SQLiteConnection con = new SQLiteConnection(conn))
            {
                con.Open();
                SQLiteCommand com = new SQLiteCommand("SELECT k.id, k.naslov, k.leto_izdaje, a.ime, a.priimek FROM knjige k INNER JOIN avtorji a ON k.avtor_id=a.id;", con);
                SQLiteDataReader reader = com.ExecuteReader();
                while (reader.Read())
                {
                    int id = reader.GetInt32(0);
                    string naslov = reader.GetString(1);
                    string leto_izdaje = reader.GetString(2);
                    string ime = reader.GetString(3);
                    string priimek = reader.GetString(4);
                    seznamGradiva.Add(new Gradivo(id, naslov, leto_izdaje, ime, priimek));
                }
                con.Close();
            }
            return seznamGradiva;
        }

        public static List<Gradivo> FilterNaslov(string naslovKnjige)
        {
            List<Gradivo> seznamGradiva = new List<Gradivo>();

            using (SQLiteConnection con = new SQLiteConnection(conn))
            {
                con.Open();
                SQLiteCommand com = new SQLiteCommand("SELECT k.id, k.naslov, k.leto_izdaje, a.ime, a.priimek FROM knjige k INNER JOIN avtorji a ON k.avtor_id=a.id " +
                    "WHERE(k.naslov LIKE '%" + naslovKnjige + "%';", con);
                SQLiteDataReader reader = com.ExecuteReader();
                while (reader.Read())
                {
                    int id = reader.GetInt32(0);
                    string naslov = reader.GetString(1);
                    string leto_izdaje = reader.GetString(2);
                    string ime = reader.GetString(3);
                    string priimek = reader.GetString(4);
                    seznamGradiva.Add(new Gradivo(id, naslov, leto_izdaje, ime, priimek));
                }
                con.Close();
            }
            return seznamGradiva;
        }
        public static List<Gradivo> FilterAvtor(string avtorKnjige)
        {
            List<Gradivo> seznamGradiva = new List<Gradivo>();

            using (SQLiteConnection con = new SQLiteConnection(conn))
            {
                con.Open();
                SQLiteCommand com = new SQLiteCommand("SELECT k.id, k.naslov, k.leto_izdaje, a.ime, a.priimek FROM knjige k INNER JOIN avtorji a ON k.avtor_id=a.id " +
                    "WHERE(a.priimek LIKE '" + avtorKnjige + "%';", con);
                SQLiteDataReader reader = com.ExecuteReader();
                while (reader.Read())
                {
                    int id = reader.GetInt32(0);
                    string naslov = reader.GetString(1);
                    string leto_izdaje = reader.GetString(2);
                    string ime = reader.GetString(3);
                    string priimek = reader.GetString(4);
                    seznamGradiva.Add(new Gradivo(id, naslov, leto_izdaje, ime, priimek));
                }
                con.Close();
            }
            return seznamGradiva;
        }
        public static List<Gradivo> FilterZalozba(string zalozbaKnjige)
        {
            List<Gradivo> seznamGradiva = new List<Gradivo>();

            using (SQLiteConnection con = new SQLiteConnection(conn))
            {
                con.Open();
                SQLiteCommand com = new SQLiteCommand("SELECT k.id, k.naslov, k.leto_izdaje, a.ime, a.priimek FROM zalozbe z INNER JOIN knjige k ON z.id=k.zalozba_id INNER JOIN avtorji a ON k.avtor_id=a.id " +
                    "WHERE(z.ime LIKE '" + zalozbaKnjige + "%';", con);
                SQLiteDataReader reader = com.ExecuteReader();
                while (reader.Read())
                {
                    int id = reader.GetInt32(0);
                    string naslov = reader.GetString(1);
                    string leto_izdaje = reader.GetString(2);
                    string ime = reader.GetString(3);
                    string priimek = reader.GetString(4);
                    seznamGradiva.Add(new Gradivo(id, naslov, leto_izdaje, ime, priimek));
                }
                con.Close();
            }
            return seznamGradiva;
        }

        public static List<Gradivo> FilterInvSt(string invStKnjige)
        {
            List<Gradivo> seznamGradiva = new List<Gradivo>();

            using (SQLiteConnection con = new SQLiteConnection(conn))
            {
                con.Open();
                SQLiteCommand com = new SQLiteCommand("SELECT k.id, k.naslov, k.leto_izdaje, a.ime, a.priimek FROM knjige k INNER JOIN avtorji a ON k.avtor_id=a.id " +
                    "WHERE(k.invSt LIKE '" + invStKnjige + "%';", con);
                SQLiteDataReader reader = com.ExecuteReader();
                while (reader.Read())
                {
                    int id = reader.GetInt32(0);
                    string naslov = reader.GetString(1);
                    string leto_izdaje = reader.GetString(2);
                    string ime = reader.GetString(3);
                    string priimek = reader.GetString(4);
                    seznamGradiva.Add(new Gradivo(id, naslov, leto_izdaje, ime, priimek));
                }
                con.Close();
            }
            return seznamGradiva;
        }
        public static void DodajGradivo(Gradivo gradivo)
        {
            try
            {
                using (SQLiteConnection con = new SQLiteConnection(conn))
                {
                    //Fix SQL
                    con.Open();
                    SQLiteCommand com = new SQLiteCommand("INSERT INTO knjige (leto_izdaje, naslov, avtor_id, zalozba_id, inventarna_st) VALUES ('" + gradivo.LetoIzdaje + "', '" + gradivo.Naslov + "', (SELECT id FROM avtorji WHERE(ime = '" + gradivo.ImeAvtorja + "') AND (priimek = '" + gradivo.PriimekAvtorja + "')), (SELECT id FROM zalozbe WHERE(ime = '" + gradivo.ImeZalozba + "')), '" + gradivo.Id + "');", con);
                    com.ExecuteNonQuery();
                    con.Close();
                }

                System.Windows.Forms.MessageBox.Show("Gradivo uspesno dodano");
            }
            catch (Exception)
            {
                System.Windows.Forms.MessageBox.Show("Napaka pri dodajanju novega gradiva");
            }
        }

        public static List<Zalozba> VrniVseZalozbe()
        {
            List<Zalozba> seznam = new List<Zalozba>();

            using (SQLiteConnection con = new SQLiteConnection(conn))
            {
                con.Open();
                SQLiteCommand com = new SQLiteCommand("SELECT id, ime FROM zalozbe;", con);
                SQLiteDataReader reader = com.ExecuteReader();
                while (reader.Read())
                {
                    int id = reader.GetInt32(0);
                    string ime = reader.GetString(1);
                    seznam.Add(new Zalozba(id, ime));
                }
                con.Close();
            }
            return seznam;
        }
    }
}
