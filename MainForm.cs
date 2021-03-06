using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace LibraryProject
{
    public partial class MainForm : Form
    {
        int uporabnik_id = -1;
        int uredi_clana_id = -1;
        public MainForm()
        {
            InitializeComponent();
            kupljenoRadioButton1.Checked = true;
            FillZalozbeCombobox();
            FillAvtorjiComboBox();
            FillKategorijeCombobox();
            loadGradivoList();
        }

        private void MainForm_Load(object sender, EventArgs e)
        {
            loadUserList();
            loadGradivoList();
        }

        private void loadUserList()
        {
            List<Uporabniki> uporabniki = Database.izberiVseUporabnike();
            foreach (Uporabniki uporabnik in uporabniki)
            {
                claniListBox.Items.Add(uporabnik);
            }
        }

        private void claniListBox_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            if (claniListBox.SelectedIndex != -1)
            {
                //Dobi in shrani izbranega uporabnika
                Uporabniki uporabnik = (Uporabniki)claniListBox.SelectedItem;
                imePriimekLabel.Text = uporabnik.ime + " " + uporabnik.priimek;
                uporabnik_id = uporabnik.id;

                //Izpiše izposojeno gradivo izbranega uporabnika
                izposojenoGradivoListBox.Items.Clear();
                List<Izposoja> izposojenoGradivo = Database.izpisIzposojenegaGradiva(uporabnik_id);
                foreach (Izposoja izposoja in izposojenoGradivo)
                {
                    izposojenoGradivoListBox.Items.Add(izposoja);
                }

                //Izpiše vso gradivo
                vsoGradivoListBoxUpdate();

                //Prikaže vracilo/izposoja tab
                tabControl1.SelectedTab = tabControl1.TabPages["vraciloIzposojaTabPage"];
            }
        }

        private void vsoGradivoListBoxUpdate()
        {
            vsoGradivoListBox.Items.Clear();
            List<Knjiga> vseKnjige = Database.izpisVsegaGradiva();
            foreach (Knjiga knjiga1 in vseKnjige)
            {
                vsoGradivoListBox.Items.Add(knjiga1);
            }
        }

        private void izposojenoListBoxUpdate()
        {
            izposojenoGradivoListBox.Items.Clear();
            List<Izposoja> izposoje = Database.izpisIzposojenegaGradiva(uporabnik_id);
            foreach (Izposoja izposoja in izposoje)
            {
                izposojenoGradivoListBox.Items.Add(izposoja);
            }
        }

        private void imePriimekTextBox_TextChanged(object sender, EventArgs e)
        {
            if (imePriimekTextBox.Text != "")
            {
                List<Uporabniki> uporabniki = Database.IsciVseUporabnike(imePriimekTextBox.Text);
                claniListBox.Items.Clear();
                foreach (Uporabniki uporabnik in uporabniki)
                {
                    claniListBox.Items.Add(uporabnik);
                }
            }
            else
            {
                List<Uporabniki> uporabniki = Database.izberiVseUporabnike();
                claniListBox.Items.Clear();
                foreach (Uporabniki uporabnik in uporabniki)
                {
                    claniListBox.Items.Add(uporabnik);
                }
            }
        }

        private void dodajClanaButton_Click(object sender, EventArgs e)
        {
            try
            {
                Database.dodajClana(imeTextBox2.Text, priimekTextBox.Text, telefonTextBox.Text, naslovTextBox.Text, emailTextBox.Text, opombeRichTextBox.Text);
            }
            catch (Exception)
            {
                MessageBox.Show("Napaka pri dodajanju člana!");
            }
            loadUserList();
        }

        private void izposodiGradivoButton_Click(object sender, EventArgs e)
        {
            if (vsoGradivoListBox.SelectedIndex != -1)
            {
                foreach (Knjiga knjiga in vsoGradivoListBox.SelectedItems)
                {
                    Database.izposojaGradiva(knjiga, uporabnik_id);
                }
                izposojenoListBoxUpdate();
                vsoGradivoListBoxUpdate();
            }
        }

        private void vrniGradivoButton_Click(object sender, EventArgs e)
        {
            if (izposojenoGradivoListBox.SelectedIndex != -1)
            {
                foreach (Izposoja knjiga in izposojenoGradivoListBox.SelectedItems)
                {
                    Database.vraciloGradiva(knjiga);
                }
                vsoGradivoListBoxUpdate();
                izposojenoListBoxUpdate();
            }
        }
        private void loadGradivoList()
        {
            gradivoListBox.Items.Clear();
            vsoGradivoListBox.Items.Clear();
            List<Knjiga> knjige = Database.izpisVsegaGradiva();
            foreach (Knjiga knjiga in knjige)
            {
                gradivoListBox.Items.Add(knjiga);
                vsoGradivoListBox.Items.Add(knjiga);
            }
        }

        private void isciButton_Click(object sender, EventArgs e)
        {
            //TODO: Vid fix this špageti

            if(naslovTextBox2.Text.Length > 0)
            {
                gradivoListBox.Items.Clear();
                List<Gradivo> seznam = Database.FilterNaslov(naslovTextBox2.Text);
                foreach (Gradivo gradivo in seznam)
                {
                    gradivoListBox.Items.Add(gradivo);
                }
            }
            else if(avtorTextBox.Text.Length > 0)
            {
                gradivoListBox.Items.Clear();
                List<Gradivo> seznam = Database.FilterAvtor(avtorTextBox.Text);
                foreach (Gradivo gradivo in seznam)
                {
                    gradivoListBox.Items.Add(gradivo);
                }
            }
            else if(zalozbaTextBox.Text.Length > 0)
            {
                gradivoListBox.Items.Clear();
                List<Gradivo> seznam = Database.FilterZalozba(zalozbaTextBox.Text);
                foreach (Gradivo gradivo in seznam)
                {
                    gradivoListBox.Items.Add(gradivo);
                }
            }
            else if(invStTextBox.Text.Length > 0)
            {
                gradivoListBox.Items.Clear();
                List<Knjiga> seznam = Database.FilterInvSt(invStTextBox.Text);
                foreach (Knjiga knjiga in seznam)
                {
                    gradivoListBox.Items.Add(knjiga);
                }
            }
        }

        private void dodajGradivoButton_Click(object sender, EventArgs e)
        {
            if(avtorComboBox.SelectedText.Length > 0)
            {
                if (letoIzdajeGradivoTextBox.Text.Length > 0)
                {
                    if (naslovGradivoTextBox.Text.Length > 0)
                    {
                        if(invStTextBox2.Text.Length > 0)
                        {
                            if (zalozbaComboBox.SelectedIndex != -1)
                            {
                                string avtor = avtorComboBox.SelectedText;
                                string[] avtorr = avtor.Split(' ');
                                string ime = avtorr[0].Trim();
                                string priimek = avtorr[1].Trim();
                                Zalozba zalozbaa = (Zalozba)zalozbaComboBox.SelectedItem;

                                if(Database.VrniAvtorId(ime, priimek) != -1)
                                {
                                    if (kupljenoRadioButton1.Checked)
                                    {
                                        Gradivo gradivo = new Gradivo(Convert.ToInt32(invStTextBox2.Text), naslovGradivoTextBox.Text, letoIzdajeGradivoTextBox.Text, ime, priimek, zalozbaa.Id, true);
                                        Database.DodajGradivo(gradivo);
                                    }
                                    else if (kupljenoRadioButton2.Checked)
                                    {
                                        Gradivo gradivo = new Gradivo(Convert.ToInt32(invStTextBox2.Text), naslovGradivoTextBox.Text, letoIzdajeGradivoTextBox.Text, ime, priimek, zalozbaa.Id, false);
                                        Database.DodajGradivo(gradivo);
                                    }
                                }
                                else
                                {
                                    Database.VnesiAvtor(ime, priimek);

                                    if (kupljenoRadioButton1.Checked)
                                    {
                                        Gradivo gradivo = new Gradivo(Convert.ToInt32(invStTextBox2.Text), naslovGradivoTextBox.Text, letoIzdajeGradivoTextBox.Text, ime, priimek, zalozbaa.Id, true);
                                        Database.DodajGradivo(gradivo);
                                    }
                                    else if (kupljenoRadioButton2.Checked)
                                    {
                                        Gradivo gradivo = new Gradivo(Convert.ToInt32(invStTextBox2.Text), naslovGradivoTextBox.Text, letoIzdajeGradivoTextBox.Text, ime, priimek, zalozbaa.Id, false);
                                        Database.DodajGradivo(gradivo);
                                    }
                                }

                                //Update list boxes
                                loadGradivoList();
                                loadUserList();
                            }
                        }
                    }
                }
            }
        }

        public void FillAvtorjiComboBox()
        {
            List<Avtor> seznam = Database.VrniVseAvtorje();

            foreach (Avtor avtor in seznam)
            {
                avtorComboBox.Items.Add(avtor);
            }
        }

        public void FillZalozbeCombobox()
        {
            List<Zalozba> seznam = Database.VrniVseZalozbe();

            foreach(Zalozba zalozba in seznam)
            {
                zalozbaComboBox.Items.Add(zalozba);
            }
        }

        public void FillKategorijeCombobox()
        {
            List<string> seznam = Database.VrniVseKategorije();
            kategorijaGradivoComboBox.Items.Clear();
            foreach (string kategorija in seznam)
            {
                kategorijaGradivoComboBox.Items.Add(kategorija);
            }
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {
            if (textBox1.Text != "")
            {
                List<Knjiga> knjiga = Database.FilterInvSt(textBox1.Text);
                vsoGradivoListBox.Items.Clear();
                foreach (Knjiga knjiga1 in knjiga)
                {
                    vsoGradivoListBox.Items.Add(knjiga1);
                }
            }
            else
            {
                List<Knjiga> knjiga = Database.izpisVsegaGradiva();
                vsoGradivoListBox.Items.Clear();
                foreach (Knjiga knjiga1 in knjiga)
                {
                    vsoGradivoListBox.Items.Add(knjiga1);
                }
            }
        }

        private void pocistiClaniButton_Click(object sender, EventArgs e)
        {
            imeTextBox2.Clear();
            priimekTextBox.Clear();
            telefonTextBox.Clear();
            naslovTextBox.Clear();
            emailTextBox.Clear();
            opombeRichTextBox.Clear();
            uredi_clana_id = -1;
            pocistiClaniButton.Enabled = false;
            urediClanaButton.Enabled = false;
        }

        private void urediClanaButton_Click(object sender, EventArgs e)
        {
            Uporabniki uporabnik = new Uporabniki(uredi_clana_id, imeTextBox2.Text, priimekTextBox.Text, telefonTextBox.Text, naslovTextBox.Text, emailTextBox.Text, opombeRichTextBox.Text);
            Database.posodobiClana(uporabnik);
            imeTextBox2.Clear();
            priimekTextBox.Clear();
            telefonTextBox.Clear();
            naslovTextBox.Clear();
            emailTextBox.Clear();
            opombeRichTextBox.Clear();
            uredi_clana_id = -1;
            pocistiClaniButton.Enabled = false;
            urediClanaButton.Enabled = false;
        }

        private void claniListBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (claniListBox.SelectedIndex != -1)
            {
                Uporabniki uporabnik = (Uporabniki)claniListBox.SelectedItem;
                int id = uporabnik.id;
                Uporabniki uporabnikk = Database.VrniUporabnika(id);

                imeLabel.Text = uporabnikk.ime;
                priimekLabel.Text = uporabnikk.priimek;
                telefonLabel.Text = uporabnikk.telefon;
                naslovLabel.Text = uporabnikk.naslov;
                emailLabel.Text = uporabnikk.email;
                opombeLabel.Text = uporabnikk.opombe;
            }
        }
    }
}