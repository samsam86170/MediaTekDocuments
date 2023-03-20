using System;
using System.Windows.Forms;
using MediaTekDocuments.model;
using MediaTekDocuments.controller;
using System.Collections.Generic;
using System.Linq;
using System.Drawing;
using System.IO;

namespace MediaTekDocuments.view

{
    /// <summary>
    /// Classe d'affichage
    /// </summary>
    public partial class FrmMediatek : Form
    {
        #region Commun
        private readonly FrmMediatekController controller;
        private readonly BindingSource bdgGenres = new BindingSource();
        private readonly BindingSource bdgPublics = new BindingSource();
        private readonly BindingSource bdgRayons = new BindingSource();

        /// <summary>
        /// Constructeur : création du contrôleur lié à ce formulaire
        /// </summary>
        internal FrmMediatek()
        {
            InitializeComponent();
            this.controller = new FrmMediatekController();
            FrmAlerteFinAbonnement frmAlerteFinAbonnement = new FrmAlerteFinAbonnement(controller);
            frmAlerteFinAbonnement.ShowDialog();
        }

        /// <summary>
        /// Rempli un des 3 combo (genre, public, rayon)
        /// </summary>
        /// <param name="lesCategories">liste des objets de type Genre ou Public ou Rayon</param>
        /// <param name="bdg">bindingsource contenant les informations</param>
        /// <param name="cbx">combobox à remplir</param>
        public void RemplirComboCategorie(List<Categorie> lesCategories, BindingSource bdg, ComboBox cbx)
        {
            bdg.DataSource = lesCategories;
            cbx.DataSource = bdg;
            if (cbx.Items.Count > 0)
            {
                cbx.SelectedIndex = -1;
            }
        }
        #endregion

        #region Onglet Livres
        private readonly BindingSource bdgLivresListe = new BindingSource();
        private List<Livre> lesLivres = new List<Livre>();

        /// <summary>
        /// Ouverture de l'onglet Livres : 
        /// appel des méthodes pour remplir le datagrid des livres et des combos (genre, rayon, public)
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void TabLivres_Enter(object sender, EventArgs e)
        {
            lesLivres = controller.GetAllLivres();
            RemplirComboCategorie(controller.GetAllGenres(), bdgGenres, cbxLivresGenres);
            RemplirComboCategorie(controller.GetAllPublics(), bdgPublics, cbxLivresPublics);
            RemplirComboCategorie(controller.GetAllRayons(), bdgRayons, cbxLivresRayons);
            RemplirLivresListeComplete();
            gbxExemplairesLivre.Enabled = false;
            gbxEtatExemplaireLivre.Enabled = false;
        }

        /// <summary>
        /// Remplit le dategrid avec la liste reçue en paramètre
        /// </summary>
        /// <param name="livres">liste de livres</param>
        private void RemplirLivresListe(List<Livre> livres)
        {
            bdgLivresListe.DataSource = livres;
            dgvLivresListe.DataSource = bdgLivresListe;
            dgvLivresListe.Columns["isbn"].Visible = false;
            dgvLivresListe.Columns["idRayon"].Visible = false;
            dgvLivresListe.Columns["idGenre"].Visible = false;
            dgvLivresListe.Columns["idPublic"].Visible = false;
            dgvLivresListe.Columns["image"].Visible = false;
            dgvLivresListe.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.AllCells;
            dgvLivresListe.Columns["id"].DisplayIndex = 0;
            dgvLivresListe.Columns["titre"].DisplayIndex = 1;
        }

        /// <summary>
        /// Recherche et affichage du livre dont on a saisi le numéro.
        /// Si non trouvé, affichage d'un MessageBox.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BtnLivresNumRecherche_Click(object sender, EventArgs e)
        {
            if (!txbLivresNumRecherche.Text.Equals(""))
            {
                txbLivresTitreRecherche.Text = "";
                cbxLivresGenres.SelectedIndex = -1;
                cbxLivresRayons.SelectedIndex = -1;
                cbxLivresPublics.SelectedIndex = -1;
                Livre livre = lesLivres.Find(x => x.Id.Equals(txbLivresNumRecherche.Text));
                if (livre != null)
                {
                    List<Livre> livres = new List<Livre>() { livre };
                    RemplirLivresListe(livres);
                    gbxExemplairesLivre.Enabled = true;
                    AfficheExemplairesLivres();
                }
                else
                {
                    MessageBox.Show("numéro introuvable");
                    RemplirLivresListeComplete();
                }
            }
            else
            {
                RemplirLivresListeComplete();
            }
        }

        /// <summary>
        /// Recherche et affichage des livres dont le titre matche acec la saisie.
        /// Cette procédure est exécutée à chaque ajout ou suppression de caractère
        /// dans le textBox de saisie.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void TxbLivresTitreRecherche_TextChanged(object sender, EventArgs e)
        {
            if (!txbLivresTitreRecherche.Text.Equals(""))
            {
                cbxLivresGenres.SelectedIndex = -1;
                cbxLivresRayons.SelectedIndex = -1;
                cbxLivresPublics.SelectedIndex = -1;
                txbLivresNumRecherche.Text = "";
                List<Livre> lesLivresParTitre;
                lesLivresParTitre = lesLivres.FindAll(x => x.Titre.ToLower().Contains(txbLivresTitreRecherche.Text.ToLower()));
                RemplirLivresListe(lesLivresParTitre);
            }
            else
            {
                // si la zone de saisie est vide et aucun élément combo sélectionné, réaffichage de la liste complète
                if (cbxLivresGenres.SelectedIndex < 0 && cbxLivresPublics.SelectedIndex < 0 && cbxLivresRayons.SelectedIndex < 0
                    && txbLivresNumRecherche.Text.Equals(""))
                {
                    RemplirLivresListeComplete();
                }
            }
        }

        /// <summary>
        /// Affichage des informations du livre sélectionné
        /// </summary>
        /// <param name="livre">le livre</param>
        private void AfficheLivresInfos(Livre livre)
        {
            txbLivresAuteur.Text = livre.Auteur;
            txbLivresCollection.Text = livre.Collection;
            txbLivresImage.Text = livre.Image;
            txbLivresIsbn.Text = livre.Isbn;
            txbLivresNumero.Text = livre.Id;
            txbLivresTitre.Text = livre.Titre;
            txbLivresGenre.Text = livre.Genre;
            txbLivresPublic.Text = livre.Public;
            txbLivresRayon.Text = livre.Rayon;
            txbIdGenreLivre.Text = livre.IdGenre;
            txbIdPublicLivre.Text = livre.IdPublic;
            txbIdRayonLivre.Text = livre.IdRayon;
            string image = livre.Image;
            try
            {
                pcbLivresImage.Image = Image.FromFile(image);
            }
            catch
            {
                pcbLivresImage.Image = null;
            }
        }

        /// <summary>
        /// Vide les zones d'affichage des informations du livre
        /// </summary>
        private void VideLivresInfos()
        {
            txbLivresAuteur.Text = "";
            txbLivresCollection.Text = "";
            txbLivresImage.Text = "";
            txbLivresIsbn.Text = "";
            txbLivresNumero.Text = "";
            txbLivresGenre.Text = "";
            txbLivresPublic.Text = "";
            txbLivresRayon.Text = "";
            txbLivresTitre.Text = "";
            txbIdGenreLivre.Text = "";
            txbIdPublicLivre.Text = "";
            txbIdRayonLivre.Text = "";
            pcbLivresImage.Image = null;
        }

        private void btnInfosLivreVider_Click(object sender, EventArgs e)
        {
            VideLivresInfos();
        }

        /// <summary>
        /// Filtre sur le genre
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void CbxLivresGenres_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cbxLivresGenres.SelectedIndex >= 0)
            {
                txbLivresTitreRecherche.Text = "";
                txbLivresNumRecherche.Text = "";
                Genre genre = (Genre)cbxLivresGenres.SelectedItem;
                List<Livre> livres = lesLivres.FindAll(x => x.Genre.Equals(genre.Libelle));
                RemplirLivresListe(livres);
                cbxLivresRayons.SelectedIndex = -1;
                cbxLivresPublics.SelectedIndex = -1;
            }
        }


        /// <summary>
        /// Filtre sur la catégorie de public
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void CbxLivresPublics_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cbxLivresPublics.SelectedIndex >= 0)
            {
                txbLivresTitreRecherche.Text = "";
                txbLivresNumRecherche.Text = "";
                Public lePublic = (Public)cbxLivresPublics.SelectedItem;
                List<Livre> livres = lesLivres.FindAll(x => x.Public.Equals(lePublic.Libelle));
                RemplirLivresListe(livres);
                cbxLivresRayons.SelectedIndex = -1;
                cbxLivresGenres.SelectedIndex = -1;
            }
        }

        /// <summary>
        /// Filtre sur le rayon
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void CbxLivresRayons_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cbxLivresRayons.SelectedIndex >= 0)
            {
                txbLivresTitreRecherche.Text = "";
                txbLivresNumRecherche.Text = "";
                Rayon rayon = (Rayon)cbxLivresRayons.SelectedItem;
                List<Livre> livres = lesLivres.FindAll(x => x.Rayon.Equals(rayon.Libelle));
                RemplirLivresListe(livres);
                cbxLivresGenres.SelectedIndex = -1;
                cbxLivresPublics.SelectedIndex = -1;
            }
        }

        /// <summary>
        /// Sur la sélection d'une ligne ou cellule dans le grid
        /// affichage des informations du livre
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void DgvLivresListe_SelectionChanged(object sender, EventArgs e)
        {
            if (dgvLivresListe.CurrentCell != null)
            {
                try
                {
                    Livre livre = (Livre)bdgLivresListe.List[bdgLivresListe.Position];
                    AfficheLivresInfos(livre);
                }
                catch
                {
                    VideLivresZones();
                }
            }
            else
            {
                VideLivresInfos();
            }
        }

        /// <summary>
        /// Sur le clic du bouton d'annulation, affichage de la liste complète des livres
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BtnLivresAnnulPublics_Click(object sender, EventArgs e)
        {
            RemplirLivresListeComplete();
        }

        /// <summary>
        /// Sur le clic du bouton d'annulation, affichage de la liste complète des livres
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BtnLivresAnnulRayons_Click(object sender, EventArgs e)
        {
            RemplirLivresListeComplete();
        }

        /// <summary>
        /// Sur le clic du bouton d'annulation, affichage de la liste complète des livres
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BtnLivresAnnulGenres_Click(object sender, EventArgs e)
        {
            RemplirLivresListeComplete();
        }

        /// <summary>
        /// Affichage de la liste complète des livres
        /// et annulation de toutes les recherches et filtres
        /// </summary>
        private void RemplirLivresListeComplete()
        {
            RemplirLivresListe(lesLivres);
            VideLivresZones();
        }

        /// <summary>
        /// vide les zones de recherche et de filtre
        /// </summary>
        private void VideLivresZones()
        {
            cbxLivresGenres.SelectedIndex = -1;
            cbxLivresRayons.SelectedIndex = -1;
            cbxLivresPublics.SelectedIndex = -1;
            txbLivresNumRecherche.Text = "";
            txbLivresTitreRecherche.Text = "";
        }

        /// <summary>
        /// Tri sur les colonnes
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void DgvLivresListe_ColumnHeaderMouseClick(object sender, DataGridViewCellMouseEventArgs e)
        {
            VideLivresZones();
            string titreColonne = dgvLivresListe.Columns[e.ColumnIndex].HeaderText;
            List<Livre> sortedList = new List<Livre>();
            switch (titreColonne)
            {
                case "Id":
                    sortedList = lesLivres.OrderBy(o => o.Id).ToList();
                    break;
                case "Titre":
                    sortedList = lesLivres.OrderBy(o => o.Titre).ToList();
                    break;
                case "Collection":
                    sortedList = lesLivres.OrderBy(o => o.Collection).ToList();
                    break;
                case "Auteur":
                    sortedList = lesLivres.OrderBy(o => o.Auteur).ToList();
                    break;
                case "Genre":
                    sortedList = lesLivres.OrderBy(o => o.Genre).ToList();
                    break;
                case "Public":
                    sortedList = lesLivres.OrderBy(o => o.Public).ToList();
                    break;
                case "Rayon":
                    sortedList = lesLivres.OrderBy(o => o.Rayon).ToList();
                    break;
            }
            RemplirLivresListe(sortedList);
        }

      
        /// <summary>
        /// Enregistrement d'un document de type "livre" dans la base de données
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnReceptionLivreAjouter_Click(object sender, EventArgs e)
        {
            if (!txbLivresNumero.Text.Equals(""))
            {
                try
                {
                    string id = txbLivresNumero.Text;
                    string titre = txbLivresTitre.Text;
                    string image = txbLivresImage.Text;
                    string isbn = txbLivresIsbn.Text;
                    string auteur = txbLivresAuteur.Text;
                    string collection = txbLivresCollection.Text;
                    string idGenre = txbIdGenreLivre.Text;
                    string genre = txbLivresGenre.Text;
                    string idPublic = txbIdPublicLivre.Text;
                    string Public = txbLivresPublic.Text;
                    string idRayon = txbIdRayonLivre.Text;
                    string rayon = txbLivresRayon.Text;

                    Document document = new Document(id, titre, image, idGenre, genre, idPublic, Public, idRayon, rayon);
                    Livre livre = new Livre(id, titre, image, isbn, auteur, collection, idGenre, genre, idPublic, Public, idRayon, rayon);
                    if (controller.CreerDocument(document.Id, document.Titre, document.Image, document.IdRayon, document.IdPublic, document.IdGenre) && controller.CreerLivre(livre.Id, livre.Isbn, livre.Auteur, livre.Collection))
                    {
                        lesLivres = controller.GetAllLivres();
                        RemplirComboCategorie(controller.GetAllGenres(), bdgGenres, cbxLivresGenres);
                        RemplirComboCategorie(controller.GetAllPublics(), bdgPublics, cbxLivresPublics);
                        RemplirComboCategorie(controller.GetAllRayons(), bdgRayons, cbxLivresRayons);
                        RemplirLivresListeComplete();
                        MessageBox.Show("Le livre " + titre + " a correctement été ajouté.");
                    }
                    else
                    {
                        MessageBox.Show("numéro de document déjà existant", "Erreur");
                    }
                }
                catch
                {
                    MessageBox.Show("Une erreur s'est produite", "Erreur");
                }
            }
            else
            {
                MessageBox.Show("Le numéro de document est obligatoire.", "Information");
            }
        }
        /// <summary>
        /// Modification d'un document de type "livre", dans la bdd
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnReceptionLivreModifier_Click(object sender, EventArgs e)
        {
            if (dgvLivresListe.SelectedRows.Count > 0)
            {
                Livre selectedLivre = (Livre)dgvLivresListe.SelectedRows[0].DataBoundItem;

                string id = selectedLivre.Id;
                string titre = txbLivresTitre.Text;
                string image = txbLivresImage.Text;
                string isbn = txbLivresIsbn.Text;
                string auteur = txbLivresAuteur.Text;
                string collection = txbLivresCollection.Text;
                string idGenre = txbIdGenreLivre.Text;
                string idPublic = txbIdPublicLivre.Text;
                string idRayon = txbIdRayonLivre.Text;
                if (!txbLivresNumero.Equals("") && !txbLivresTitre.Equals("") && !txbIdGenreLivre.Equals("") && !txbIdPublicLivre.Equals("") && !txbIdRayonLivre.Equals(""))
                {
                    if (controller.ModifierLivre(id, isbn, auteur, collection) && controller.ModifierDocument(id, titre, image, idGenre, idPublic, idRayon))
                    {
                        lesLivres = controller.GetAllLivres();
                        RemplirComboCategorie(controller.GetAllGenres(), bdgGenres, cbxLivresGenres);
                        RemplirComboCategorie(controller.GetAllPublics(), bdgPublics, cbxLivresPublics);
                        RemplirComboCategorie(controller.GetAllRayons(), bdgRayons, cbxLivresRayons);
                        RemplirLivresListeComplete();
                        MessageBox.Show("Le livre " + titre + " a correctement été modifié.");
                    }
                    else
                    {
                        MessageBox.Show("Erreur lors de la modification du LIVRE", "Erreur");
                    }
                }
                else
                {
                    MessageBox.Show("Veuillez sélectionner un livre à modifier", "Information");
                }
            }
            else
            {
                MessageBox.Show("Le numéro de document est obligatoire.", "Information");
            }
        }

        /// <summary>
        /// Suppression d'un document de type "livre" en bdd
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnSupprimerLivre_Click(object sender, EventArgs e)
        {
            Livre livre = (Livre)bdgLivresListe.Current;
           
            if (MessageBox.Show("Souhaitez-vous confirmer la suppression?", "Confirmation de suppression", MessageBoxButtons.OKCancel) == DialogResult.OK)
            {
                if (controller.SupprimerLivre(livre.Id))
                {
                    lesLivres = controller.GetAllLivres();
                    RemplirComboCategorie(controller.GetAllGenres(), bdgGenres, cbxLivresGenres);
                    RemplirComboCategorie(controller.GetAllPublics(), bdgPublics, cbxLivresPublics);
                    RemplirComboCategorie(controller.GetAllRayons(), bdgRayons, cbxLivresRayons);
                    RemplirLivresListeComplete();
                    MessageBox.Show("Le livre " + livre.Titre + " a correctement été supprimé.");
                }
                else
                {
                    MessageBox.Show("Une erreur s'est produite.", "Erreur");
                }
            }
        }

        private readonly BindingSource bdgExemplairesLivre = new BindingSource();
        private List<Exemplaire> lesExemplairesDocument = new List<Exemplaire>();

        /// <summary>
        /// Remplit la datagrid avec la liste passée en paramètre
        /// </summary>
        /// <param name="lesExemplaires"></param>
        private void RemplirExemplairesLivre(List<Exemplaire> lesExemplaires)
        {
            if (lesExemplaires != null)
            {
                bdgExemplairesLivre.DataSource = lesExemplaires;
                dgvExemplairesLivre.DataSource = bdgExemplairesLivre;
                dgvExemplairesLivre.Columns["photo"].Visible = false;
                dgvExemplairesLivre.Columns["idEtat"].Visible = false;
                dgvExemplairesLivre.Columns["id"].Visible = false;
                dgvExemplairesLivre.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.AllCells;
                dgvExemplairesLivre.Columns[0].HeaderCell.Value = "Numéro";
                dgvExemplairesLivre.Columns[2].HeaderCell.Value = "Date d'achat";
                dgvExemplairesLivre.Columns[5].HeaderCell.Value = "Etat";
            }
            else
            {
                dgvExemplairesLivre.DataSource = null;
            }
        }

        /// <summary>
        /// Affichage des exemplaires d'un livre 
        /// </summary>
        private void AfficheExemplairesLivres()
        {
            string idDocument = txbLivresNumRecherche.Text;
            lesExemplairesDocument = controller.GetExemplairesDocument(idDocument);
            RemplirExemplairesLivre(lesExemplairesDocument);
        }

        /// <summary>
        /// Tri sur les colonnes
        /// </summary>
        /// <param name="sender">sender</param>
        /// <param name="e">e</param>
        private void dgvExemplairesLivre_ColumnHeaderMouseClick(object sender, DataGridViewCellMouseEventArgs e)
        {
            string titreColonne = dgvExemplairesLivre.Columns[e.ColumnIndex].HeaderText;
            List<Exemplaire> sortedList = new List<Exemplaire>();
            switch (titreColonne)
            {
                case "Date d'achat":
                    sortedList = lesExemplairesDocument.OrderBy(o => o.DateAchat).Reverse().ToList();
                    break;
                case "Numéro":
                    sortedList = lesExemplairesDocument.OrderBy(o => o.Numero).ToList();
                    break;
                case "Etat":
                    sortedList = lesExemplairesDocument.OrderBy(o => o.Libelle).ToList();
                    break;
            }
            RemplirExemplairesLivre(sortedList);
        }

        /// <summary>
        /// Remplissage de la comboBox selon les états de l'exemplaire et le libelle correspondant
        /// </summary>
        /// <param name="etatExemplaireLivre"></param>
        private void RemplirCbxEtatLibelleExemplaireLivre(string etatExemplaireLivre)
        {
            cbxEtatLibelleExemplaireLivre.Items.Clear();
            if (etatExemplaireLivre == "neuf")
            {
                cbxEtatLibelleExemplaireLivre.Items.Add("usagé");
                cbxEtatLibelleExemplaireLivre.Items.Add("détérioré");
                cbxEtatLibelleExemplaireLivre.Items.Add("inutilisable");
            }

            else if (etatExemplaireLivre == "usagé")
            {
                cbxEtatLibelleExemplaireLivre.Text = "";
                cbxEtatLibelleExemplaireLivre.Items.Add("neuf");
                cbxEtatLibelleExemplaireLivre.Items.Add("détérioré");
                cbxEtatLibelleExemplaireLivre.Items.Add("inutilisable");
            }
            else if (etatExemplaireLivre == "détérioré")
            {
                cbxEtatLibelleExemplaireLivre.Text = "";
                cbxEtatLibelleExemplaireLivre.Items.Add("neuf");
                cbxEtatLibelleExemplaireLivre.Items.Add("usagé");
                cbxEtatLibelleExemplaireLivre.Items.Add("inutilisable");
            }
            else if (etatExemplaireLivre == "inutilisable")
            {
                cbxEtatLibelleExemplaireLivre.Text = "";
                cbxEtatLibelleExemplaireLivre.Items.Add("neuf");
                cbxEtatLibelleExemplaireLivre.Items.Add("usagé");
                cbxEtatLibelleExemplaireLivre.Items.Add("détérioré");
            }
        }

        /// <summary>
        /// Selon le libelle dans la txbBox, affichage des états possibles de l'exemplaire
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void lblEtatExemplaireLivre_TextChanged(object sender, EventArgs e)
        {
            string etatExemplaireLivre = lblEtatExemplaireLivre.Text;
            RemplirCbxEtatLibelleExemplaireLivre(etatExemplaireLivre);
        }

        /// <summary>
        /// Récupère l'id d'un état selon son libelle
        /// </summary>
        /// <param name="libelle"></param>
        /// <returns></returns>
        private string GetIdEtat(string libelle)
        {
            List<Etat> lesEtats = controller.GetAllEtatsDocument();
            foreach (Etat unEtat in lesEtats)
            {
                if (unEtat.Libelle == libelle)
                {
                    return unEtat.Id;
                }
            }
            return null;
        }

        /// <summary>
        /// Affichage des informations d'un exemplaire sélectionné
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void dgvExemplairesLivre_RowEnter(object sender, DataGridViewCellEventArgs e)
        {
            DataGridViewRow row = dgvExemplairesLivre.Rows[e.RowIndex];

            string id = row.Cells["Id"].Value.ToString();
            string numero = row.Cells["Numero"].Value.ToString();
            DateTime dateAchat = (DateTime)row.Cells["dateAchat"].Value;
            string libelle = row.Cells["Libelle"].Value.ToString();

            txbExemplaireLivresNumero.Text = numero;
            dtpDateAchatExemplaireLivre.Value = dateAchat;
            lblEtatExemplaireLivre.Text = libelle;
            gbxEtatExemplaireLivre.Enabled = true;
        }

        /// <summary>
        /// Modification de l'état d'un exemplaire de livre dans la bdd
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnEtatExemplaireLivreModifier_Click(object sender, EventArgs e)
        {
            string idDocument = txbLivresNumRecherche.Text;
            int numero = int.Parse(txbExemplaireLivresNumero.Text);
            DateTime dateAchat = dtpDateAchatExemplaireLivre.Value;
            string photo = "";
            string idEtat = GetIdEtat(cbxEtatLibelleExemplaireLivre.Text);
            try
            {
                string libelle = cbxEtatLibelleExemplaireLivre.SelectedItem.ToString();

                Exemplaire exemplaire = new Exemplaire(numero, dateAchat, photo, idEtat, idDocument, libelle);
                if (MessageBox.Show("Voulez-vous modifier l'état de l'exemplaire " + exemplaire.Numero + " en " + libelle + " ?", "Confirmation", MessageBoxButtons.YesNo) == DialogResult.Yes)
                {
                    controller.ModifierEtatExemplaireDocument(exemplaire);
                    MessageBox.Show("L'état de l'exemplaire " + exemplaire.Numero + " a bien été modifié.", "Information");
                    AfficheExemplairesLivres();
                }
            }
            catch (NullReferenceException)
            {
                MessageBox.Show("Le nouvel état de l'exemplaire doit être sélectionné.", "Information");
            }
        }

        /// <summary>
        /// Suppression d'un exemplaire de livre dans la bdd
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnExemplaireLivreSupprimer_Click(object sender, EventArgs e)
        {
            if (dgvExemplairesLivre.SelectedRows.Count > 0)
            {
                Exemplaire exemplaire = (Exemplaire)bdgExemplairesLivre.List[bdgExemplairesLivre.Position];
                if (MessageBox.Show("Voulez-vous supprimer l'exemplaire " + exemplaire.Numero + " du livre " + exemplaire.Id + " ?", "Confirmation de suppression", MessageBoxButtons.YesNo) == DialogResult.Yes)
                {
                    controller.SupprimerExemplaireDocument(exemplaire);
                    MessageBox.Show("L'exemplaire " + exemplaire.Numero + " a bien été supprimé.", "Information");
                    AfficheExemplairesLivres();
                }
            }
            else
            {
                MessageBox.Show("Une ligne doit être sélectionnée.", "Information");
            }
        }
        #endregion

        #region Onglet Dvd
        private readonly BindingSource bdgDvdListe = new BindingSource();
        private List<Dvd> lesDvd = new List<Dvd>();

        /// <summary>
        /// Ouverture de l'onglet Dvds : 
        /// appel des méthodes pour remplir le datagrid des dvd et des combos (genre, rayon, public)
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void tabDvd_Enter(object sender, EventArgs e)
        {
            lesDvd = controller.GetAllDvd();
            gbxEtatExemplaireDvd.Enabled = false;
            RemplirComboCategorie(controller.GetAllGenres(), bdgGenres, cbxDvdGenres);
            RemplirComboCategorie(controller.GetAllPublics(), bdgPublics, cbxDvdPublics);
            RemplirComboCategorie(controller.GetAllRayons(), bdgRayons, cbxDvdRayons);
            RemplirDvdListeComplete();
        }

        /// <summary>
        /// Remplit le dategrid avec la liste reçue en paramètre
        /// </summary>
        /// <param name="Dvds">liste de dvd</param>
        private void RemplirDvdListe(List<Dvd> Dvds)
        {
            bdgDvdListe.DataSource = Dvds;
            dgvDvdListe.DataSource = bdgDvdListe;
            dgvDvdListe.Columns["idRayon"].Visible = false;
            dgvDvdListe.Columns["idGenre"].Visible = false;
            dgvDvdListe.Columns["idPublic"].Visible = false;
            dgvDvdListe.Columns["image"].Visible = false;
            dgvDvdListe.Columns["synopsis"].Visible = false;
            dgvDvdListe.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.AllCells;
            dgvDvdListe.Columns["id"].DisplayIndex = 0;
            dgvDvdListe.Columns["titre"].DisplayIndex = 1;
        }

        /// <summary>
        /// Recherche et affichage du Dvd dont on a saisi le numéro.
        /// Si non trouvé, affichage d'un MessageBox.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnDvdNumRecherche_Click(object sender, EventArgs e)
        {
            if (!txbDvdNumRecherche.Text.Equals(""))
            {
                txbDvdTitreRecherche.Text = "";
                cbxDvdGenres.SelectedIndex = -1;
                cbxDvdRayons.SelectedIndex = -1;
                cbxDvdPublics.SelectedIndex = -1;
                Dvd dvd = lesDvd.Find(x => x.Id.Equals(txbDvdNumRecherche.Text));
                if (dvd != null)
                {
                    List<Dvd> Dvd = new List<Dvd>() { dvd };
                    RemplirDvdListe(Dvd);
                    AfficheExemplairesDvd();
                    gbxEtatExemplaireDvd.Enabled = true;
                }
                else
                {
                    MessageBox.Show("numéro introuvable");
                    RemplirDvdListeComplete();
                }
            }
            else
            {
                RemplirDvdListeComplete();
            }
        }

        /// <summary>
        /// Recherche et affichage des Dvd dont le titre matche acec la saisie.
        /// Cette procédure est exécutée à chaque ajout ou suppression de caractère
        /// dans le textBox de saisie.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void txbDvdTitreRecherche_TextChanged(object sender, EventArgs e)
        {
            if (!txbDvdTitreRecherche.Text.Equals(""))
            {
                cbxDvdGenres.SelectedIndex = -1;
                cbxDvdRayons.SelectedIndex = -1;
                cbxDvdPublics.SelectedIndex = -1;
                txbDvdNumRecherche.Text = "";
                List<Dvd> lesDvdParTitre;
                lesDvdParTitre = lesDvd.FindAll(x => x.Titre.ToLower().Contains(txbDvdTitreRecherche.Text.ToLower()));
                RemplirDvdListe(lesDvdParTitre);
            }
            else
            {
                // si la zone de saisie est vide et aucun élément combo sélectionné, réaffichage de la liste complète
                if (cbxDvdGenres.SelectedIndex < 0 && cbxDvdPublics.SelectedIndex < 0 && cbxDvdRayons.SelectedIndex < 0
                    && txbDvdNumRecherche.Text.Equals(""))
                {
                    RemplirDvdListeComplete();
                }
            }
        }

        /// <summary>
        /// Affichage des informations du dvd sélectionné
        /// </summary>
        /// <param name="dvd">le dvd</param>
        private void AfficheDvdInfos(Dvd dvd)
        {
            txbDvdRealisateur.Text = dvd.Realisateur;
            txbDvdSynopsis.Text = dvd.Synopsis;
            txbDvdImage.Text = dvd.Image;
            txbDvdDuree.Text = dvd.Duree.ToString();
            txbDvdNumero.Text = dvd.Id;
            txbDvdGenre.Text = dvd.Genre;
            txbDvdPublic.Text = dvd.Public;
            txbDvdRayon.Text = dvd.Rayon;
            txbDvdTitre.Text = dvd.Titre;
            txbIdGenreDvd.Text = dvd.IdGenre;
            txbIdPublicDvd.Text = dvd.IdPublic;
            txbIdRayonDvd.Text = dvd.IdRayon;
            string image = dvd.Image;
            try
            {
                pcbDvdImage.Image = Image.FromFile(image);
            }
            catch
            {
                pcbDvdImage.Image = null;
            }
        }

        /// <summary>
        /// Vide les zones d'affichage des informations du dvd
        /// </summary>
        private void VideDvdInfos()
        {
            txbDvdRealisateur.Text = "";
            txbDvdSynopsis.Text = "";
            txbDvdImage.Text = "";
            txbDvdDuree.Text = "";
            txbDvdNumero.Text = "";
            txbDvdGenre.Text = "";
            txbDvdPublic.Text = "";
            txbDvdRayon.Text = "";
            txbDvdTitre.Text = "";
            txbIdGenreDvd.Text = "";
            txbIdPublicDvd.Text = "";
            txbIdRayonDvd.Text = "";
            pcbDvdImage.Image = null;
        }

        private void btnInfosDvdVider_Click(object sender, EventArgs e)
        {
            VideDvdInfos();
        }

        /// <summary>
        /// Filtre sur le genre
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void cbxDvdGenres_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cbxDvdGenres.SelectedIndex >= 0)
            {
                txbDvdTitreRecherche.Text = "";
                txbDvdNumRecherche.Text = "";
                Genre genre = (Genre)cbxDvdGenres.SelectedItem;
                List<Dvd> Dvd = lesDvd.FindAll(x => x.Genre.Equals(genre.Libelle));
                RemplirDvdListe(Dvd);
                cbxDvdRayons.SelectedIndex = -1;
                cbxDvdPublics.SelectedIndex = -1;
            }
        }

        /// <summary>
        /// Filtre sur la catégorie de public
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void cbxDvdPublics_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cbxDvdPublics.SelectedIndex >= 0)
            {
                txbDvdTitreRecherche.Text = "";
                txbDvdNumRecherche.Text = "";
                Public lePublic = (Public)cbxDvdPublics.SelectedItem;
                List<Dvd> Dvd = lesDvd.FindAll(x => x.Public.Equals(lePublic.Libelle));
                RemplirDvdListe(Dvd);
                cbxDvdRayons.SelectedIndex = -1;
                cbxDvdGenres.SelectedIndex = -1;
            }
        }

        /// <summary>
        /// Filtre sur le rayon
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void cbxDvdRayons_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cbxDvdRayons.SelectedIndex >= 0)
            {
                txbDvdTitreRecherche.Text = "";
                txbDvdNumRecherche.Text = "";
                Rayon rayon = (Rayon)cbxDvdRayons.SelectedItem;
                List<Dvd> Dvd = lesDvd.FindAll(x => x.Rayon.Equals(rayon.Libelle));
                RemplirDvdListe(Dvd);
                cbxDvdGenres.SelectedIndex = -1;
                cbxDvdPublics.SelectedIndex = -1;
            }
        }

        /// <summary>
        /// Sur la sélection d'une ligne ou cellule dans le grid
        /// affichage des informations du dvd
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void dgvDvdListe_SelectionChanged(object sender, EventArgs e)
        {
            if (dgvDvdListe.CurrentCell != null)
            {
                try
                {
                    Dvd dvd = (Dvd)bdgDvdListe.List[bdgDvdListe.Position];
                    AfficheDvdInfos(dvd);
                }
                catch
                {
                    VideDvdZones();
                }
            }
            else
            {
                VideDvdInfos();
            }
        }

        /// <summary>
        /// Sur le clic du bouton d'annulation, affichage de la liste complète des Dvd
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnDvdAnnulPublics_Click(object sender, EventArgs e)
        {
            RemplirDvdListeComplete();
        }

        /// <summary>
        /// Sur le clic du bouton d'annulation, affichage de la liste complète des Dvd
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnDvdAnnulRayons_Click(object sender, EventArgs e)
        {
            RemplirDvdListeComplete();
        }

        /// <summary>
        /// Sur le clic du bouton d'annulation, affichage de la liste complète des Dvd
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnDvdAnnulGenres_Click(object sender, EventArgs e)
        {
            RemplirDvdListeComplete();
        }

        /// <summary>
        /// Affichage de la liste complète des Dvd
        /// et annulation de toutes les recherches et filtres
        /// </summary>
        private void RemplirDvdListeComplete()
        {
            RemplirDvdListe(lesDvd);
            VideDvdZones();
        }

        /// <summary>
        /// vide les zones de recherche et de filtre
        /// </summary>
        private void VideDvdZones()
        {
            cbxDvdGenres.SelectedIndex = -1;
            cbxDvdRayons.SelectedIndex = -1;
            cbxDvdPublics.SelectedIndex = -1;
            txbDvdNumRecherche.Text = "";
            txbDvdTitreRecherche.Text = "";
        }

        /// <summary>
        /// Tri sur les colonnes
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void dgvDvdListe_ColumnHeaderMouseClick(object sender, DataGridViewCellMouseEventArgs e)
        {
            VideDvdZones();
            string titreColonne = dgvDvdListe.Columns[e.ColumnIndex].HeaderText;
            List<Dvd> sortedList = new List<Dvd>();
            switch (titreColonne)
            {
                case "Id":
                    sortedList = lesDvd.OrderBy(o => o.Id).ToList();
                    break;
                case "Titre":
                    sortedList = lesDvd.OrderBy(o => o.Titre).ToList();
                    break;
                case "Duree":
                    sortedList = lesDvd.OrderBy(o => o.Duree).ToList();
                    break;
                case "Realisateur":
                    sortedList = lesDvd.OrderBy(o => o.Realisateur).ToList();
                    break;
                case "Genre":
                    sortedList = lesDvd.OrderBy(o => o.Genre).ToList();
                    break;
                case "Public":
                    sortedList = lesDvd.OrderBy(o => o.Public).ToList();
                    break;
                case "Rayon":
                    sortedList = lesDvd.OrderBy(o => o.Rayon).ToList();
                    break;
            }
            RemplirDvdListe(sortedList);
        }

        /// <summary>
        /// Enregistrement d'un document de type "dvd" dans la base de données
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnReceptionDvdValider_Click(object sender, EventArgs e)
        {
            if (!txbDvdNumero.Text.Equals(""))
            {
                try
                {
                    string id = txbDvdNumero.Text;
                    string titre = txbDvdTitre.Text;
                    string image = txbDvdImage.Text;
                    int duree = int.Parse(txbDvdDuree.Text);
                    string realisateur = txbDvdRealisateur.Text;
                    string synopsis = txbDvdSynopsis.Text;
                    string idGenre = txbIdGenreDvd.Text;
                    string genre = txbDvdGenre.Text;
                    string idPublic = txbIdPublicDvd.Text;
                    string lePublic = txbDvdPublic.Text;
                    string idRayon = txbIdRayonDvd.Text;
                    string rayon = txbDvdRayon.Text;

                    Document document = new Document(id, titre, image, idGenre, genre, idPublic, lePublic, idRayon, rayon);
                    Dvd dvd = new Dvd(id, titre, image, duree, realisateur, synopsis, idGenre, genre, idPublic, lePublic, idRayon, rayon);
                    if (controller.CreerDocument(document.Id, document.Titre, document.Image, document.IdRayon, document.IdPublic, document.IdGenre) && controller.CreerDvd(dvd.Id, dvd.Synopsis, dvd.Realisateur, dvd.Duree))
                    {
                        lesDvd = controller.GetAllDvd();
                        RemplirComboCategorie(controller.GetAllGenres(), bdgGenres, cbxDvdGenres);
                        RemplirComboCategorie(controller.GetAllPublics(), bdgPublics, cbxDvdPublics);
                        RemplirComboCategorie(controller.GetAllRayons(), bdgRayons, cbxDvdRayons);
                        RemplirDvdListeComplete();
                        MessageBox.Show("Le dvd " + titre + " a correctement été ajouté.");
                    }
                    else
                    {
                        MessageBox.Show("numéro de document déjà existant", "Erreur");
                    }
                }
                catch
                {
                    MessageBox.Show("Une erreur s'est produite", "Erreur");
                }
            }
            else
            {
                MessageBox.Show("numéro de document obligatoire", "Information");
            }
        }

        /// <summary>
        /// Modification d'un document de type "dvd", dans la bdd
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnReceptionDvdModifier_Click(object sender, EventArgs e)
        {
            if (!txbDvdNumero.Text.Equals(""))
            {

                if (dgvDvdListe.SelectedRows.Count > 0)
                {
                    Dvd selectedDvd = (Dvd)dgvDvdListe.SelectedRows[0].DataBoundItem;
                    
                    string id = selectedDvd.Id;
                    string synopsis = txbDvdSynopsis.Text;
                    string realisateur = txbDvdRealisateur.Text;
                    int duree = int.Parse(txbDvdDuree.Text);
                    string titre = txbDvdTitre.Text;
                    string image = txbDvdImage.Text;
                    string idGenre = txbIdGenreDvd.Text;
                    string idPublic = txbIdPublicDvd.Text;
                    string idRayon = txbIdRayonDvd.Text;
                    
                    if (controller.ModifierDvd(id, synopsis, realisateur, duree) && controller.ModifierDocument(id, titre, image, idGenre, idPublic, idRayon))
                    {
                        lesDvd = controller.GetAllDvd();
                        RemplirComboCategorie(controller.GetAllGenres(), bdgGenres, cbxDvdGenres);
                        RemplirComboCategorie(controller.GetAllPublics(), bdgPublics, cbxDvdPublics);
                        RemplirComboCategorie(controller.GetAllRayons(), bdgRayons, cbxDvdRayons);
                        RemplirDvdListeComplete();
                        MessageBox.Show("Le dvd " + titre + " a correctement été modifié.");
                    }
                    else
                    {
                        MessageBox.Show("Erreur lors de la modification du DVD", "Erreur");
                    }
                }
                else
                {
                    MessageBox.Show("Veuillez sélectionner un DVD à modifier", "Information");
                }
            }
        }

        /// <summary>
        /// Suppression d'un document de type "dvd" en bdd
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnSupprimerDvd_Click(object sender, EventArgs e)
        {
            Dvd dvd = (Dvd)bdgDvdListe.Current;
            if (MessageBox.Show("Souhaitez-vous confirmer la suppression?", "Confirmation de suppression", MessageBoxButtons.OKCancel) == DialogResult.OK)
            {
                if (controller.SupprimerDvd(dvd.Id))
                {
                    lesDvd = controller.GetAllDvd();
                    RemplirComboCategorie(controller.GetAllGenres(), bdgGenres, cbxDvdGenres);
                    RemplirComboCategorie(controller.GetAllPublics(), bdgPublics, cbxDvdPublics);
                    RemplirComboCategorie(controller.GetAllRayons(), bdgRayons, cbxDvdRayons);
                    RemplirDvdListeComplete();
                    MessageBox.Show("Le dvd " + dvd.Titre + " a correctement été supprimé.");
                }
                else
                {
                    MessageBox.Show("Une erreur s'est produite.", "Erreur");
                }
            }
        }

        private readonly BindingSource bdgExemplairesDvd = new BindingSource();

        /// <summary>
        /// Remplit la datagrid avec la liste passée en paramètre
        /// </summary>
        /// <param name="lesExemplaires"></param>
        private void RemplirExemplairesDvd(List<Exemplaire> lesExemplaires)
        {
            if (lesExemplaires != null)
            {
                bdgExemplairesDvd.DataSource = lesExemplaires;
                dgvExemplairesDvd.DataSource = bdgExemplairesDvd;
                dgvExemplairesDvd.Columns["photo"].Visible = false;
                dgvExemplairesDvd.Columns["idEtat"].Visible = false;
                dgvExemplairesDvd.Columns["id"].Visible = false;
                dgvExemplairesDvd.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.AllCells;
                dgvExemplairesDvd.Columns[0].HeaderCell.Value = "Numéro";
                dgvExemplairesDvd.Columns[2].HeaderCell.Value = "Date d'achat";
                dgvExemplairesDvd.Columns[5].HeaderCell.Value = "Etat";
            }
            else
            {
                dgvExemplairesDvd.DataSource = null;
            }
        }

        /// <summary>
        /// Affichage des exemplaires d'un dvd
        /// </summary>
        private void AfficheExemplairesDvd()
        {
            string idDocument = txbDvdNumRecherche.Text;
            lesExemplairesDocument = controller.GetExemplairesDocument(idDocument);
            RemplirExemplairesDvd(lesExemplairesDocument);
        }

        /// <summary>
        /// Tri sur les colonnes
        /// </summary>
        /// <param name="sender">sender</param>
        /// <param name="e">e</param>
        private void dgvExemplairesDvd_ColumnHeaderMouseClick(object sender, DataGridViewCellMouseEventArgs e)
        {
            string titreColonne = dgvExemplairesDvd.Columns[e.ColumnIndex].HeaderText;
            List<Exemplaire> sortedList = new List<Exemplaire>();
            switch (titreColonne)
            {
                case "Date d'achat":
                    sortedList = lesExemplairesDocument.OrderBy(o => o.DateAchat).Reverse().ToList();
                    break;
                case "Numéro":
                    sortedList = lesExemplairesDocument.OrderBy(o => o.Numero).ToList();
                    break;
                case "Etat":
                    sortedList = lesExemplairesDocument.OrderBy(o => o.Libelle).ToList();
                    break;
            }
            RemplirExemplairesDvd(sortedList);
        }

        /// <summary>
        /// Remplissage de la comboBox selon les états de l'exemplaire et le libelle correspondant
        /// </summary>
        /// <param name="etatExemplaireDvd"></param>
        private void RemplirCbxEtatLibelleExemplaireDvd(string etatExemplaireDvd)
        {
            cbxEtatLibelleExemplaireDvd.Items.Clear();
            if (etatExemplaireDvd == "neuf")
            {
                cbxEtatLibelleExemplaireDvd.Items.Add("usagé");
                cbxEtatLibelleExemplaireDvd.Items.Add("détérioré");
                cbxEtatLibelleExemplaireDvd.Items.Add("inutilisable");
            }

            else if (etatExemplaireDvd == "usagé")
            {
                cbxEtatLibelleExemplaireDvd.Text = "";
                cbxEtatLibelleExemplaireDvd.Items.Add("neuf");
                cbxEtatLibelleExemplaireDvd.Items.Add("détérioré");
                cbxEtatLibelleExemplaireDvd.Items.Add("inutilisable");
            }
            else if (etatExemplaireDvd == "détérioré")
            {
                cbxEtatLibelleExemplaireDvd.Text = "";
                cbxEtatLibelleExemplaireDvd.Items.Add("neuf");
                cbxEtatLibelleExemplaireDvd.Items.Add("usagé");
                cbxEtatLibelleExemplaireDvd.Items.Add("inutilisable");
            }
            else if (etatExemplaireDvd == "inutilisable")
            {
                cbxEtatLibelleExemplaireDvd.Text = "";
                cbxEtatLibelleExemplaireDvd.Items.Add("neuf");
                cbxEtatLibelleExemplaireDvd.Items.Add("usagé");
                cbxEtatLibelleExemplaireDvd.Items.Add("détérioré");
            }
        }

        /// <summary>
        /// Selon le libelle dans la txbBox, affichage des états possibles de l'exemplaire
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void lblEtatExemplaireDvd_TextChanged(object sender, EventArgs e)
        {
            string etatExemplaireDvd = lblEtatExemplaireDvd.Text;
            RemplirCbxEtatLibelleExemplaireDvd(etatExemplaireDvd);
        }

        /// <summary>
        /// Affichage des informations d'un exemplaire sélectionné
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void dgvExemplairesDvd_RowEnter(object sender, DataGridViewCellEventArgs e)
        {
            DataGridViewRow row = dgvExemplairesDvd.Rows[e.RowIndex];

            string id = row.Cells["Id"].Value.ToString();
            string numero = row.Cells["Numero"].Value.ToString();
            DateTime dateAchat = (DateTime)row.Cells["dateAchat"].Value;
            string libelle = row.Cells["Libelle"].Value.ToString();

            txbExemplaireDvdNumero.Text = numero;
            dtpDateAchatExemplaireDvd.Value = dateAchat;
            lblEtatExemplaireDvd.Text = libelle;
            gbxEtatExemplaireDvd.Enabled = true;
        }

        /// <summary>
        /// Modification de l'état d'un exemplaire de dvd dans la bdd
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnEtatExemplaireDvdModifier_Click(object sender, EventArgs e)
        {
            string idDocument = txbDvdNumRecherche.Text;
            int numero = int.Parse(txbExemplaireDvdNumero.Text);
            DateTime dateAchat = dtpDateAchatExemplaireDvd.Value;
            string photo = "";
            string idEtat = GetIdEtat(cbxEtatLibelleExemplaireDvd.Text);
            try
            {
                string libelle = cbxEtatLibelleExemplaireDvd.SelectedItem.ToString();

                Exemplaire exemplaire = new Exemplaire(numero, dateAchat, photo, idEtat, idDocument, libelle);
                if (MessageBox.Show("Voulez-vous modifier l'état de l'exemplaire " + exemplaire.Numero + " en " + libelle + " ?", "Confirmation", MessageBoxButtons.YesNo) == DialogResult.Yes)
                {
                    controller.ModifierEtatExemplaireDocument(exemplaire);
                    MessageBox.Show("L'état de l'exemplaire " + exemplaire.Numero + " a bien été modifié.", "Information");
                    AfficheExemplairesDvd();
                }
            }
            catch (NullReferenceException)
            {
                MessageBox.Show("Le nouvel état de l'exemplaire doit être sélectionné.", "Information");
            }
        }

        /// <summary>
        /// Suppression d'un exemplaire de dvd dans la bdd
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnExemplaireDvdSupprimer_Click(object sender, EventArgs e)
        {
            if (dgvExemplairesDvd.SelectedRows.Count > 0)
            {
                Exemplaire exemplaire = (Exemplaire)bdgExemplairesDvd.List[bdgExemplairesDvd.Position];
                if (MessageBox.Show("Voulez-vous supprimer l'exemplaire " + exemplaire.Numero + " du dvd " + exemplaire.Id + " ?", "Confirmation de suppression", MessageBoxButtons.YesNo) == DialogResult.Yes)
                {
                    controller.SupprimerExemplaireDocument(exemplaire);
                    MessageBox.Show("L'exemplaire " + exemplaire.Numero + " a bien été supprimé.", "Information");
                    AfficheExemplairesDvd();
                }
            }
            else
            {
                MessageBox.Show("Une ligne doit être sélectionnée.", "Information");
            }
        }
        #endregion

        #region Onglet Revues
        private readonly BindingSource bdgRevuesListe = new BindingSource();
        private List<Revue> lesRevues = new List<Revue>();

        /// <summary>
        /// Ouverture de l'onglet Revues : 
        /// appel des méthodes pour remplir le datagrid des revues et des combos (genre, rayon, public)
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void tabRevues_Enter(object sender, EventArgs e)
        {
            lesRevues = controller.GetAllRevues();
            RemplirComboCategorie(controller.GetAllGenres(), bdgGenres, cbxRevuesGenres);
            RemplirComboCategorie(controller.GetAllPublics(), bdgPublics, cbxRevuesPublics);
            RemplirComboCategorie(controller.GetAllRayons(), bdgRayons, cbxRevuesRayons);
            RemplirRevuesListeComplete();
        }

        /// <summary>
        /// Remplit le dategrid avec la liste reçue en paramètre
        /// </summary>
        /// <param name="revues"></param>
        private void RemplirRevuesListe(List<Revue> revues)
        {
            bdgRevuesListe.DataSource = revues;
            dgvRevuesListe.DataSource = bdgRevuesListe;
            dgvRevuesListe.Columns["idRayon"].Visible = false;
            dgvRevuesListe.Columns["idGenre"].Visible = false;
            dgvRevuesListe.Columns["idPublic"].Visible = false;
            dgvRevuesListe.Columns["image"].Visible = false;
            dgvRevuesListe.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.AllCells;
            dgvRevuesListe.Columns["id"].DisplayIndex = 0;
            dgvRevuesListe.Columns["titre"].DisplayIndex = 1;
        }

        /// <summary>
        /// Recherche et affichage de la revue dont on a saisi le numéro.
        /// Si non trouvé, affichage d'un MessageBox.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnRevuesNumRecherche_Click(object sender, EventArgs e)
        {
            if (!txbRevuesNumRecherche.Text.Equals(""))
            {
                txbRevuesTitreRecherche.Text = "";
                cbxRevuesGenres.SelectedIndex = -1;
                cbxRevuesRayons.SelectedIndex = -1;
                cbxRevuesPublics.SelectedIndex = -1;
                Revue revue = lesRevues.Find(x => x.Id.Equals(txbRevuesNumRecherche.Text));
                if (revue != null)
                {
                    List<Revue> revues = new List<Revue>() { revue };
                    RemplirRevuesListe(revues);
                }
                else
                {
                    MessageBox.Show("numéro introuvable");
                    RemplirRevuesListeComplete();
                }
            }
            else
            {
                RemplirRevuesListeComplete();
            }
        }

        /// <summary>
        /// Recherche et affichage des revues dont le titre matche acec la saisie.
        /// Cette procédure est exécutée à chaque ajout ou suppression de caractère
        /// dans le textBox de saisie.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void txbRevuesTitreRecherche_TextChanged(object sender, EventArgs e)
        {
            if (!txbRevuesTitreRecherche.Text.Equals(""))
            {
                cbxRevuesGenres.SelectedIndex = -1;
                cbxRevuesRayons.SelectedIndex = -1;
                cbxRevuesPublics.SelectedIndex = -1;
                txbRevuesNumRecherche.Text = "";
                List<Revue> lesRevuesParTitre;
                lesRevuesParTitre = lesRevues.FindAll(x => x.Titre.ToLower().Contains(txbRevuesTitreRecherche.Text.ToLower()));
                RemplirRevuesListe(lesRevuesParTitre);
            }
            else
            {
                // si la zone de saisie est vide et aucun élément combo sélectionné, réaffichage de la liste complète
                if (cbxRevuesGenres.SelectedIndex < 0 && cbxRevuesPublics.SelectedIndex < 0 && cbxRevuesRayons.SelectedIndex < 0
                    && txbRevuesNumRecherche.Text.Equals(""))
                {
                    RemplirRevuesListeComplete();
                }
            }
        }

        /// <summary>
        /// Affichage des informations de la revue sélectionné
        /// </summary>
        /// <param name="revue">la revue</param>
        private void AfficheRevuesInfos(Revue revue)
        {
            txbRevuesPeriodicite.Text = revue.Periodicite;
            txbRevuesImage.Text = revue.Image;
            txbRevuesDateMiseADispo.Text = revue.DelaiMiseADispo.ToString();
            txbRevuesNumero.Text = revue.Id;
            txbRevuesGenre.Text = revue.Genre;
            txbRevuesPublic.Text = revue.Public;
            txbRevuesRayon.Text = revue.Rayon;
            txbRevuesTitre.Text = revue.Titre;
            txbIdGenreRevue.Text = revue.IdGenre;
            txbIdPublicRevue.Text = revue.IdPublic;
            txbIdRayonRevue.Text = revue.IdRayon;
            string image = revue.Image;
            try
            {
                pcbRevuesImage.Image = Image.FromFile(image);
            }
            catch
            {
                pcbRevuesImage.Image = null;
            }
        }

        /// <summary>
        /// Vide les zones d'affichage des informations de la reuve
        /// </summary>
        private void VideRevuesInfos()
        {
            txbRevuesPeriodicite.Text = "";
            txbRevuesImage.Text = "";
            txbRevuesDateMiseADispo.Text = "";
            txbRevuesNumero.Text = "";
            txbRevuesGenre.Text = "";
            txbRevuesPublic.Text = "";
            txbRevuesRayon.Text = "";
            txbRevuesTitre.Text = "";
            txbIdGenreRevue.Text = "";
            txbIdPublicRevue.Text = "";
            txbIdRayonRevue.Text = "";
            pcbRevuesImage.Image = null;
        }

        private void btnInfosRevuesVider_Click(object sender, EventArgs e)
        {
            VideRevuesInfos();
        }

        /// <summary>
        /// Filtre sur le genre
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void cbxRevuesGenres_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cbxRevuesGenres.SelectedIndex >= 0)
            {
                txbRevuesTitreRecherche.Text = "";
                txbRevuesNumRecherche.Text = "";
                Genre genre = (Genre)cbxRevuesGenres.SelectedItem;
                List<Revue> revues = lesRevues.FindAll(x => x.Genre.Equals(genre.Libelle));
                RemplirRevuesListe(revues);
                cbxRevuesRayons.SelectedIndex = -1;
                cbxRevuesPublics.SelectedIndex = -1;
            }
        }

        /// <summary>
        /// Filtre sur la catégorie de public
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void cbxRevuesPublics_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cbxRevuesPublics.SelectedIndex >= 0)
            {
                txbRevuesTitreRecherche.Text = "";
                txbRevuesNumRecherche.Text = "";
                Public lePublic = (Public)cbxRevuesPublics.SelectedItem;
                List<Revue> revues = lesRevues.FindAll(x => x.Public.Equals(lePublic.Libelle));
                RemplirRevuesListe(revues);
                cbxRevuesRayons.SelectedIndex = -1;
                cbxRevuesGenres.SelectedIndex = -1;
            }
        }

        /// <summary>
        /// Filtre sur le rayon
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void cbxRevuesRayons_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cbxRevuesRayons.SelectedIndex >= 0)
            {
                txbRevuesTitreRecherche.Text = "";
                txbRevuesNumRecherche.Text = "";
                Rayon rayon = (Rayon)cbxRevuesRayons.SelectedItem;
                List<Revue> revues = lesRevues.FindAll(x => x.Rayon.Equals(rayon.Libelle));
                RemplirRevuesListe(revues);
                cbxRevuesGenres.SelectedIndex = -1;
                cbxRevuesPublics.SelectedIndex = -1;
            }
        }

        /// <summary>
        /// Sur la sélection d'une ligne ou cellule dans le grid
        /// affichage des informations de la revue
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void dgvRevuesListe_SelectionChanged(object sender, EventArgs e)
        {
            if (dgvRevuesListe.CurrentCell != null)
            {
                try
                {
                    Revue revue = (Revue)bdgRevuesListe.List[bdgRevuesListe.Position];
                    AfficheRevuesInfos(revue);
                }
                catch
                {
                    VideRevuesZones();
                }
            }
            else
            {
                VideRevuesInfos();
            }
        }

        /// <summary>
        /// Sur le clic du bouton d'annulation, affichage de la liste complète des revues
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnRevuesAnnulPublics_Click(object sender, EventArgs e)
        {
            RemplirRevuesListeComplete();
        }

        /// <summary>
        /// Sur le clic du bouton d'annulation, affichage de la liste complète des revues
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnRevuesAnnulRayons_Click(object sender, EventArgs e)
        {
            RemplirRevuesListeComplete();
        }

        /// <summary>
        /// Sur le clic du bouton d'annulation, affichage de la liste complète des revues
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnRevuesAnnulGenres_Click(object sender, EventArgs e)
        {
            RemplirRevuesListeComplete();
        }

        /// <summary>
        /// Affichage de la liste complète des revues
        /// et annulation de toutes les recherches et filtres
        /// </summary>
        private void RemplirRevuesListeComplete()
        {
            RemplirRevuesListe(lesRevues);
            VideRevuesZones();
        }

        /// <summary>
        /// vide les zones de recherche et de filtre
        /// </summary>
        private void VideRevuesZones()
        {
            cbxRevuesGenres.SelectedIndex = -1;
            cbxRevuesRayons.SelectedIndex = -1;
            cbxRevuesPublics.SelectedIndex = -1;
            txbRevuesNumRecherche.Text = "";
            txbRevuesTitreRecherche.Text = "";
        }

        /// <summary>
        /// Tri sur les colonnes
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void dgvRevuesListe_ColumnHeaderMouseClick(object sender, DataGridViewCellMouseEventArgs e)
        {
            VideRevuesZones();
            string titreColonne = dgvRevuesListe.Columns[e.ColumnIndex].HeaderText;
            List<Revue> sortedList = new List<Revue>();
            switch (titreColonne)
            {
                case "Id":
                    sortedList = lesRevues.OrderBy(o => o.Id).ToList();
                    break;
                case "Titre":
                    sortedList = lesRevues.OrderBy(o => o.Titre).ToList();
                    break;
                case "Periodicite":
                    sortedList = lesRevues.OrderBy(o => o.Periodicite).ToList();
                    break;
                case "DelaiMiseADispo":
                    sortedList = lesRevues.OrderBy(o => o.DelaiMiseADispo).ToList();
                    break;
                case "Genre":
                    sortedList = lesRevues.OrderBy(o => o.Genre).ToList();
                    break;
                case "Public":
                    sortedList = lesRevues.OrderBy(o => o.Public).ToList();
                    break;
                case "Rayon":
                    sortedList = lesRevues.OrderBy(o => o.Rayon).ToList();
                    break;
            }
            RemplirRevuesListe(sortedList);
        }

        /// <summary>
        /// Enregistrement d'un document de type "revue" en bdd
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnReceptionRevueValider_Click(object sender, EventArgs e)
        {
            if (!txbRevuesNumero.Text.Equals("") && !txbRevuesTitre.Equals("") && !txbIdGenreRevue.Equals("") && !txbIdPublicRevue.Equals("") && !txbIdRayonRevue.Equals(""))
            {
                string id = txbRevuesNumero.Text;
                string titre = txbRevuesTitre.Text;
                string image = txbRevuesImage.Text;
                string idGenre = txbIdGenreRevue.Text;
                string genre = txbRevuesGenre.Text;
                string idPublic = txbIdPublicRevue.Text;
                string lePublic = txbRevuesPublic.Text;
                string idRayon = txbIdRayonRevue.Text;
                string rayon = txbRevuesRayon.Text;
                string periodicite = txbRevuesPeriodicite.Text;
                int delaiMiseADispo = int.Parse(txbRevuesDateMiseADispo.Text);

                Document document = new Document(id, titre, image, idGenre, genre, idPublic, lePublic, idRayon, rayon);
                Revue revue = new Revue(id, titre, image, idGenre, genre, idPublic, lePublic, idRayon, rayon, periodicite, delaiMiseADispo);

                if (controller.CreerDocument(document.Id, document.Titre, document.Image, document.IdRayon, document.IdPublic, document.IdGenre) && controller.CreerRevue(revue.Id, revue.Periodicite, revue.DelaiMiseADispo))
                {
                    lesRevues = controller.GetAllRevues();
                    RemplirComboCategorie(controller.GetAllGenres(), bdgGenres, cbxRevuesGenres);
                    RemplirComboCategorie(controller.GetAllPublics(), bdgPublics, cbxRevuesPublics);
                    RemplirComboCategorie(controller.GetAllRayons(), bdgRayons, cbxRevuesRayons);
                    RemplirRevuesListeComplete();
                    MessageBox.Show("La revue " + titre + " a correctement été ajoutée.");
                }
                else
                {
                    MessageBox.Show("Une erreur s'est produite", "Erreur");
                }
            }
            else
            {
                MessageBox.Show("Le champ : numéro de document est obligatoire.", "Information");
            }
        }

        /// <summary>
        /// Modification d'un document de type "revue" en bdd
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnReceptionRevueModifier_Click(object sender, EventArgs e)
        {
            if (dgvRevuesListe.SelectedRows.Count > 0)
            {
                Revue selectedRevue = (Revue)dgvRevuesListe.SelectedRows[0].DataBoundItem;

                string id = selectedRevue.Id;
                string titre = txbRevuesTitre.Text;
                string image = txbRevuesImage.Text;
                string idGenre = txbIdGenreRevue.Text;
                string idPublic = txbIdPublicRevue.Text;
                string idRayon = txbIdRayonRevue.Text;
                string periodicite = txbRevuesPeriodicite.Text;
                int delaiMiseADispo = int.Parse(txbRevuesDateMiseADispo.Text);
                if (!txbRevuesNumero.Equals("") && !txbRevuesTitre.Equals("") && !txbIdGenreRevue.Equals("") && !txbIdPublicRevue.Equals("") && !txbIdRayonRevue.Equals(""))
                {
                    if (controller.ModifierRevue(id, periodicite, delaiMiseADispo) && controller.ModifierDocument(id, titre, image, idGenre, idPublic, idRayon))
                    {
                        dgvRevuesListe.DataSource = controller.GetAllRevues();
                        MessageBox.Show("La revue " + titre + " a correctement été modifiée.");
                    }
                    else
                    {
                        MessageBox.Show("Erreur lors de la modification de la REVUE", "Erreur");
                    }
                }
                else
                {
                    MessageBox.Show("Veuillez sélectionner une REVUE à modifier", "Information");
                }
            }
            else
            {
                MessageBox.Show("Le champ : numéro de document est obligatoire.", "Information");
            }
        }

        /// <summary>
        /// Suppression d'un document de type "revue" en bdd
        /// La revue ne peut être supprimée que si elle ne contient pas d'exemplaires
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnSupprimerRevue_Click(object sender, EventArgs e)
        {
            Revue revue = (Revue)bdgRevuesListe.Current;
            if (MessageBox.Show("Souhaitez-vous confirmer la suppression?", "Confirmation de suppression", MessageBoxButtons.OKCancel) == DialogResult.OK)
            {
                List<Exemplaire> lesExemplaires = controller.GetExemplairesRevue(revue.Id);
                bool aucunExemplaire = true;
                foreach (Exemplaire exemplaire in lesExemplaires)
                {
                    if (exemplaire.Id == revue.Id)
                    {
                        aucunExemplaire = false;
                        break;
                    }
                }
                if (aucunExemplaire)
                {
                    if (controller.SupprimerRevue(revue.Id))
                    {
                        lesRevues = controller.GetAllRevues();
                        RemplirComboCategorie(controller.GetAllGenres(), bdgGenres, cbxRevuesGenres);
                        RemplirComboCategorie(controller.GetAllPublics(), bdgPublics, cbxRevuesPublics);
                        RemplirComboCategorie(controller.GetAllRayons(), bdgRayons, cbxRevuesRayons);
                        RemplirRevuesListeComplete();
                        MessageBox.Show("La revue " + revue.Titre + " a correctement été supprimée.");
                    }
                    else
                    {
                        MessageBox.Show("Une erreur s'est produite.", "Erreur");
                    }
                }
                else
                {
                    MessageBox.Show("Impossible de supprimer la revue car elle a un ou plusieurs exemplaires associés.", "Erreur");
                }
            }
        }

        #endregion

        #region Onglet Paarutions
        private readonly BindingSource bdgExemplairesListe = new BindingSource();
        private List<Exemplaire> lesExemplaires = new List<Exemplaire>();
        const string ETATNEUF = "00001";

        /// <summary>
        /// Ouverture de l'onglet : récupère le revues et vide tous les champs.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void tabReceptionRevue_Enter(object sender, EventArgs e)
        {
            lesRevues = controller.GetAllRevues();
            txbReceptionRevueNumero.Text = "";
            gbxEtatExemplaireRevue.Enabled = false;
        }

        /// <summary>
        /// Remplit le dategrid des exemplaires avec la liste reçue en paramètre
        /// </summary>
        /// <param name="exemplaires">liste d'exemplaires</param>
        private void RemplirReceptionExemplairesListe(List<Exemplaire> exemplaires)
        {
            if (exemplaires != null)
            {
                bdgExemplairesListe.DataSource = exemplaires;
                dgvReceptionExemplairesListe.DataSource = bdgExemplairesListe;
                dgvReceptionExemplairesListe.Columns["idEtat"].Visible = false;
                dgvReceptionExemplairesListe.Columns["id"].Visible = false;
                dgvReceptionExemplairesListe.Columns["photo"].Visible = false;
                dgvReceptionExemplairesListe.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.AllCells;
                dgvReceptionExemplairesListe.Columns[1].HeaderCell.Value = "Numéro";
                dgvReceptionExemplairesListe.Columns[3].HeaderCell.Value = "Date d'achat";
                dgvReceptionExemplairesListe.Columns[5].HeaderCell.Value = "Etat";
            }
            else
            {
                bdgExemplairesListe.DataSource = null;
            }
        }

        /// <summary>
        /// Recherche d'un numéro de revue et affiche ses informations
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnReceptionRechercher_Click(object sender, EventArgs e)
        {
            if (!txbReceptionRevueNumero.Text.Equals(""))
            {
                Revue revue = lesRevues.Find(x => x.Id.Equals(txbReceptionRevueNumero.Text));
                if (revue != null)
                {
                    AfficheReceptionRevueInfos(revue);
                }
                else
                {
                    MessageBox.Show("numéro introuvable");
                }
            }
        }

        /// <summary>
        /// Si le numéro de revue est modifié, la zone de l'exemplaire est vidée et inactive
        /// les informations de la revue son aussi effacées
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void txbReceptionRevueNumero_TextChanged(object sender, EventArgs e)
        {
            txbReceptionRevuePeriodicite.Text = "";
            txbReceptionRevueImage.Text = "";
            txbReceptionRevueDelaiMiseADispo.Text = "";
            txbReceptionRevueGenre.Text = "";
            txbReceptionRevuePublic.Text = "";
            txbReceptionRevueRayon.Text = "";
            txbReceptionRevueTitre.Text = "";
            pcbReceptionRevueImage.Image = null;
            RemplirReceptionExemplairesListe(null);
            AccesReceptionExemplaireGroupBox(false);
        }

        /// <summary>
        /// Affichage des informations de la revue sélectionnée et les exemplaires
        /// </summary>
        /// <param name="revue">la revue</param>
        private void AfficheReceptionRevueInfos(Revue revue)
        {
            // informations sur la revue
            txbReceptionRevuePeriodicite.Text = revue.Periodicite;
            txbReceptionRevueImage.Text = revue.Image;
            txbReceptionRevueDelaiMiseADispo.Text = revue.DelaiMiseADispo.ToString();
            txbReceptionRevueNumero.Text = revue.Id;
            txbReceptionRevueGenre.Text = revue.Genre;
            txbReceptionRevuePublic.Text = revue.Public;
            txbReceptionRevueRayon.Text = revue.Rayon;
            txbReceptionRevueTitre.Text = revue.Titre;
            string image = revue.Image;
            try
            {
                pcbReceptionRevueImage.Image = Image.FromFile(image);
            }
            catch
            {
                pcbReceptionRevueImage.Image = null;
            }
            // affiche la liste des exemplaires de la revue
            AfficheReceptionExemplairesRevue();
        }

        /// <summary>
        /// Récupère et affiche les exemplaires d'une revue
        /// </summary>
        private void AfficheReceptionExemplairesRevue()
        {
            string idDocument = txbReceptionRevueNumero.Text;
            lesExemplaires = controller.GetExemplairesDocument(idDocument);
            RemplirReceptionExemplairesListe(lesExemplaires);
            AccesReceptionExemplaireGroupBox(true);
        }

        /// <summary>
        /// Permet ou interdit l'accès à la gestion de la réception d'un exemplaire
        /// et vide les objets graphiques
        /// </summary>
        /// <param name="acces">true ou false</param>
        private void AccesReceptionExemplaireGroupBox(bool acces)
        {
            grpReceptionExemplaire.Enabled = acces;
            txbReceptionExemplaireImage.Text = "";
            txbReceptionExemplaireNumero.Text = "";
            pcbReceptionExemplaireImage.Image = null;
            dtpReceptionExemplaireDate.Value = DateTime.Now;
        }

        /// <summary>
        /// Recherche image sur disque (pour l'exemplaire à insérer)
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnReceptionExemplaireImage_Click(object sender, EventArgs e)
        {
            string filePath = "";
            OpenFileDialog openFileDialog = new OpenFileDialog()
            {
                // positionnement à la racine du disque où se trouve le dossier actuel
                InitialDirectory = Path.GetPathRoot(Environment.CurrentDirectory),
                Filter = "Files|*.jpg;*.bmp;*.jpeg;*.png;*.gif"
            };
            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                filePath = openFileDialog.FileName;
            }
            txbReceptionExemplaireImage.Text = filePath;
            try
            {
                pcbReceptionExemplaireImage.Image = Image.FromFile(filePath);
            }
            catch
            {
                pcbReceptionExemplaireImage.Image = null;
            }
        }

        /// <summary>
        /// Enregistrement du nouvel exemplaire
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnReceptionExemplaireValider_Click(object sender, EventArgs e)
        {
            if (!txbReceptionExemplaireNumero.Text.Equals(""))
            {
                try
                {
                    int numero = int.Parse(txbReceptionExemplaireNumero.Text);
                    DateTime dateAchat = dtpReceptionExemplaireDate.Value;
                    string photo = txbReceptionExemplaireImage.Text;
                    string idEtat = ETATNEUF;
                    string idDocument = txbReceptionRevueNumero.Text;
                    string libelle = "";
                    Exemplaire exemplaire = new Exemplaire(numero, dateAchat, photo, idEtat, idDocument, libelle);
                    if (controller.CreerExemplaireRevue(idDocument, numero, dateAchat, photo, idEtat))
                    {
                        AfficheReceptionExemplairesRevue();
                    }
                    else
                    {
                        MessageBox.Show("numéro de publication déjà existant", "Erreur");
                    }
                }
                catch
                {
                    MessageBox.Show("le numéro de parution doit être numérique", "Information");
                    txbReceptionExemplaireNumero.Text = "";
                    txbReceptionExemplaireNumero.Focus();
                }
            }
            else
            {
                MessageBox.Show("numéro de parution obligatoire", "Information");
            }
        }

        /// <summary>
        /// Tri sur une colonne
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void dgvExemplairesListe_ColumnHeaderMouseClick(object sender, DataGridViewCellMouseEventArgs e)
        {
            string titreColonne = dgvReceptionExemplairesListe.Columns[e.ColumnIndex].HeaderText;
            List<Exemplaire> sortedList = new List<Exemplaire>();
            switch (titreColonne)
            {
                case "Numero":
                    sortedList = lesExemplaires.OrderBy(o => o.Numero).Reverse().ToList();
                    break;
                case "DateAchat":
                    sortedList = lesExemplaires.OrderBy(o => o.DateAchat).Reverse().ToList();
                    break;
                case "Etat":
                    sortedList = lesExemplaires.OrderBy(o => o.Libelle).ToList();
                    break;
            }
            RemplirReceptionExemplairesListe(sortedList);
        }

        /// <summary>
        /// affichage de l'image de l'exemplaire suite à la sélection d'un exemplaire dans la liste
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void dgvReceptionExemplairesListe_SelectionChanged(object sender, EventArgs e)
        {
            if (dgvReceptionExemplairesListe.CurrentCell != null)
            {
                Exemplaire exemplaire = (Exemplaire)bdgExemplairesListe.List[bdgExemplairesListe.Position];
                string image = exemplaire.Photo;
                try
                {
                    pcbReceptionExemplaireRevueImage.Image = Image.FromFile(image);
                }
                catch
                {
                    pcbReceptionExemplaireRevueImage.Image = null;
                }
            }
            else
            {
                pcbReceptionExemplaireRevueImage.Image = null;
            }
        }

        /// <summary>
        /// Remplissage de la comboBox selon les états de l'exemplaire et le libelle correspondant
        /// </summary>
        /// <param name="etatExemplaireRevue"></param>
        private void RemplirCbxEtatLibelleExemplaireRevue(string etatExemplaireRevue)
        {
            cbxEtatLibelleExemplaireRevue.Items.Clear();
            if (etatExemplaireRevue == "neuf")
            {
                cbxEtatLibelleExemplaireRevue.Items.Add("usagé");
                cbxEtatLibelleExemplaireRevue.Items.Add("détérioré");
                cbxEtatLibelleExemplaireRevue.Items.Add("inutilisable");
            }

            else if (etatExemplaireRevue == "usagé")
            {
                cbxEtatLibelleExemplaireRevue.Text = "";
                cbxEtatLibelleExemplaireRevue.Items.Add("neuf");
                cbxEtatLibelleExemplaireRevue.Items.Add("détérioré");
                cbxEtatLibelleExemplaireRevue.Items.Add("inutilisable");
            }
            else if (etatExemplaireRevue == "détérioré")
            {
                cbxEtatLibelleExemplaireRevue.Text = "";
                cbxEtatLibelleExemplaireRevue.Items.Add("neuf");
                cbxEtatLibelleExemplaireRevue.Items.Add("usagé");
                cbxEtatLibelleExemplaireRevue.Items.Add("inutilisable");
            }
            else if (etatExemplaireRevue == "inutilisable")
            {
                cbxEtatLibelleExemplaireRevue.Text = "";
                cbxEtatLibelleExemplaireRevue.Items.Add("neuf");
                cbxEtatLibelleExemplaireRevue.Items.Add("usagé");
                cbxEtatLibelleExemplaireRevue.Items.Add("détérioré");
            }
        }

        /// <summary>
        /// Selon le libelle dans la txbBox, affichage des états possibles de l'exemplaire
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void lblEtatExemplaireRevue_TextChanged(object sender, EventArgs e)
        {
            string etatExemplaireRevue = lblEtatExemplaireRevue.Text;
            RemplirCbxEtatLibelleExemplaireRevue(etatExemplaireRevue);
        }

        /// <summary>
        /// Affichage des informations d'un exemplaire sélectionné
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void dgvReceptionExemplairesListe_RowEnter(object sender, DataGridViewCellEventArgs e)
        {
            DataGridViewRow row = dgvReceptionExemplairesListe.Rows[e.RowIndex];

            string id = row.Cells["Id"].Value.ToString();
            string numero = row.Cells["Numero"].Value.ToString();
            DateTime dateAchat = (DateTime)row.Cells["dateAchat"].Value;
            string libelle = row.Cells["Libelle"].Value.ToString();
            gbxEtatExemplaireRevue.Enabled = true;

            lblNumeroExemplaireRevue.Text = numero;
            dtpDateAchatExemplaireRevue.Value = dateAchat;
            lblEtatExemplaireRevue.Text = libelle;
        }

        /// <summary>
        /// Modification de l'état d'un exemplaire de revue dans la bdd
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnEtatExemplaireRevueModifier_Click(object sender, EventArgs e)
        {
            string idDocument = txbReceptionRevueNumero.Text;
            int numero = int.Parse(lblNumeroExemplaireRevue.Text);
            DateTime dateAchat = dtpDateAchatExemplaireRevue.Value;
            string photo = "";
            string idEtat = GetIdEtat(cbxEtatLibelleExemplaireRevue.Text);
            try
            {
                string libelle = cbxEtatLibelleExemplaireRevue.SelectedItem.ToString();

                Exemplaire exemplaire = new Exemplaire(numero, dateAchat, photo, idEtat, idDocument, libelle);
                if (MessageBox.Show("Voulez-vous modifier l'état de l'exemplaire " + exemplaire.Numero + " en " + libelle + " ?", "Confirmation", MessageBoxButtons.YesNo) == DialogResult.Yes)
                {
                    controller.ModifierEtatExemplaireDocument(exemplaire);
                    MessageBox.Show("L'état de l'exemplaire " + exemplaire.Numero + " a bien été modifié.", "Information");
                    AfficheReceptionExemplairesRevue();
                }
            }
            catch (NullReferenceException)
            {
                MessageBox.Show("Le nouvel état de l'exemplaire doit être sélectionné.", "Information");
            }
        }

        /// <summary>
        /// Suppression d'un exemplaire de revue dans la bdd
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnExemplaireRevueSupprimer_Click(object sender, EventArgs e)
        {
            if (dgvReceptionExemplairesListe.SelectedRows.Count > 0)
            {
                Exemplaire exemplaire = (Exemplaire)bdgExemplairesListe.List[bdgExemplairesListe.Position];
                if (MessageBox.Show("Voulez-vous supprimer l'exemplaire " + exemplaire.Numero + " de la revue " + exemplaire.Id + " ?", "Confirmation de suppression", MessageBoxButtons.YesNo) == DialogResult.Yes)
                {
                    controller.SupprimerExemplaireDocument(exemplaire);
                    MessageBox.Show("L'exemplaire " + exemplaire.Numero + " a bien été supprimé.", "Information");
                    AfficheReceptionExemplairesRevue();
                }
            }
            else
            {
                MessageBox.Show("Une ligne doit être sélectionnée.", "Information");
            }
        }

        #endregion

        #region Onglet CommandesLivres
        private readonly BindingSource bdgCommandesLivre = new BindingSource();
        private List<CommandeDocument> lesCommandesDocument = new List<CommandeDocument>();
        private List<Suivi> lesSuivis = new List<Suivi>();

        /// <summary>
        /// Ouverture de l'onglet Commandes de livres :
        /// appel des méthodes pour remplir le datagrid des commandes de livre et du combo "suivi"
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void TabCommandesLivres_Enter(object sender, EventArgs e)
        {
            lesLivres = controller.GetAllLivres();
            lesSuivis = controller.GetAllSuivis();
            gbxInfosCommandeLivre.Enabled = false;
            gbxEtapeSuivi.Enabled = false;
        }

        /// <summary>
        /// Masque la groupBox des suivis
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void GbxInfosCommandeLivre_Enter(object sender, EventArgs e)
        {
            gbxEtapeSuivi.Enabled = false;
        }

        /// <summary>
        /// Affiche la groupBox des suivis
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BtnInfosCommandeLivreAnnuler_Click(object sender, EventArgs e)
        {
            gbxEtapeSuivi.Enabled = true;
            gbxInfosCommandeLivre.Enabled = false;
        }

        /// <summary>
        /// Masque la groupBox des informations de commande et le numéro de recherche
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void GbxEtapeSuivi_Enter(object sender, EventArgs e)
        {
            gbxInfosCommandeLivre.Enabled = false;
            txbCommandesLivresNumRecherche.Enabled = false;
        }

        /// <summary>
        /// Affiche la groupBox des commandes et le numéro de recherche
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BtnEtapeSuiviAnnuler_Click(object sender, EventArgs e)
        {
            gbxEtapeSuivi.Enabled = false;
            gbxInfosCommandeLivre.Enabled = true;
            txbCommandesLivresNumRecherche.Enabled = true;
        }

        /// <summary>
        /// Remplit la datagrid avec la liste reçue en paramètre
        /// </summary>
        /// <param name="lesCommandesDocument"></param>
        private void RemplirCommandesLivresListe(List<CommandeDocument> lesCommandesDocument)
        {
            if (lesCommandesDocument != null)
            {
                bdgCommandesLivre.DataSource = lesCommandesDocument;
                dgvCommandesLivre.DataSource = bdgCommandesLivre;
                dgvCommandesLivre.Columns["id"].Visible = false;
                dgvCommandesLivre.Columns["idLivreDvd"].Visible = false;
                dgvCommandesLivre.Columns["idSuivi"].Visible = false;
                dgvCommandesLivre.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.AllCells;
                dgvCommandesLivre.Columns["dateCommande"].DisplayIndex = 4;
                dgvCommandesLivre.Columns["montant"].DisplayIndex = 1;
                dgvCommandesLivre.Columns[5].HeaderCell.Value = "Date de commande";
                dgvCommandesLivre.Columns[0].HeaderCell.Value = "Nombre d'exemplaires";
                dgvCommandesLivre.Columns[3].HeaderCell.Value = "Suivi";
            }
            else
            {
                bdgCommandesLivre.DataSource = null;
            }
        }

        /// <summary>
        /// Mise à jour de la liste des commandes de livre
        /// </summary>
        private void AfficheReceptionCommandesLivre()
        {
            string idDocument = txbCommandesLivresNumRecherche.Text;
            lesCommandesDocument = controller.GetCommandesDocument(idDocument);
            RemplirCommandesLivresListe(lesCommandesDocument);
        }

        /// <summary>
        /// Recherche et affichage du livre dont on a saisi le numéro.
        /// Si non trouvé, affichage d'un MessageBox.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnCommandesLivresNumRecherche_Click(object sender, EventArgs e)
        {
            if (!txbCommandesLivresNumRecherche.Text.Equals(""))
            {
                Livre livre = lesLivres.Find(x => x.Id.Equals(txbCommandesLivresNumRecherche.Text));
                if (livre != null)
                {
                    AfficheReceptionCommandesLivre();
                    gbxInfosCommandeLivre.Enabled = true;
                    AfficheReceptionCommandesLivreInfos(livre);
                }
                else
                {
                    MessageBox.Show("numéro introuvable");
                }
            }
            else
            {
                MessageBox.Show("Le numéro de document est obligatoire", "Information");
            }
        }

        /// <summary>
        /// Affichage des informations du livre
        /// </summary>
        /// <param name="livre">Le livre</param>
        private void AfficheReceptionCommandesLivreInfos(Livre livre)
        {
            txbCommandeLivresTitre.Text = livre.Titre;
            txbCommandeLivresAuteur.Text = livre.Auteur;
            txbCommandeLivresIsbn.Text = livre.Isbn;
            txbCommandeLivresCollection.Text = livre.Collection;
            txbCommandeLivresGenre.Text = livre.Genre;
            txbCommandeLivresPublic.Text = livre.Public;
            txbCommandeLivresRayon.Text = livre.Rayon;
            txbCommandeLivresCheminImage.Text = livre.Image;
            string image = livre.Image;
            try
            {
                pictureBox1.Image = Image.FromFile(image);
            }
            catch
            {
                pictureBox1.Image = null;
            }
            AfficheReceptionCommandesLivre();
        }

        /// <summary>
        /// Selon le libelle dans la txbBox, affichage des étapes de suivi correspondantes
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void lblEtapeSuivi_TextChanged(object sender, EventArgs e)
        {
            string etapeSuivi = lblEtapeSuivi.Text;
            RemplirCbxCommandeLivreLibelleSuivi(etapeSuivi);
        }

        /// <summary>
        /// Remplissage de la comboBox selon les étapes de suivi et le libelle correspondant
        /// </summary>
        /// <param name="etapeSuivi"></param>
        private void RemplirCbxCommandeLivreLibelleSuivi(string etapeSuivi)
        {
            cbxCommandeLivresLibelleSuivi.Items.Clear();
            if (etapeSuivi == "livrée")
            {
                cbxCommandeLivresLibelleSuivi.Text = "";
                cbxCommandeLivresLibelleSuivi.Items.Add("réglée");
            }
            else if (etapeSuivi == "en cours")
            {
                cbxCommandeLivresLibelleSuivi.Text = "";
                cbxCommandeLivresLibelleSuivi.Items.Add("relancée");
                cbxCommandeLivresLibelleSuivi.Items.Add("livrée");
            }
            else if (etapeSuivi == "relancée")
            {
                cbxCommandeLivresLibelleSuivi.Text = "";
                cbxCommandeLivresLibelleSuivi.Items.Add("en cours");
                cbxCommandeLivresLibelleSuivi.Items.Add("livrée");
            }
        }

        /// <summary>
        /// Récupération de l'id de suivi d'une commande selon son libelle
        /// </summary>
        /// <param name="libelle"></param>
        /// <returns></returns>
        private string GetIdSuivi(string libelle)
        {
            List<Suivi> lesSuivis = controller.GetAllSuivis();
            foreach (Suivi unSuivi in lesSuivis)
            {
                if (unSuivi.Libelle == libelle)
                {
                    return unSuivi.Id;
                }
            }
            return null;
        }

        /// <summary>
        /// Affichage des informations de la commande sélectionnée 
        /// Masque le bouton "Modifier étape de suivi" si étape finale
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void dgvCommandeslivre_RowEnter(object sender, DataGridViewCellEventArgs e)
        {
            DataGridViewRow row = dgvCommandesLivre.Rows[e.RowIndex];

            string id = row.Cells["Id"].Value.ToString();
            DateTime dateCommande = (DateTime)row.Cells["dateCommande"].Value;
            double montant = double.Parse(row.Cells["Montant"].Value.ToString());
            int nbExemplaire = int.Parse(row.Cells["NbExemplaire"].Value.ToString());
            string libelle = row.Cells["Libelle"].Value.ToString();

            txbCommandeLivreNumero.Text = id;
            txbCommandeLivreNbExemplaires.Text = nbExemplaire.ToString();
            txbCommandeLivreMontant.Text = montant.ToString();
            dtpCommandeLivre.Value = dateCommande;
            lblEtapeSuivi.Text = libelle;

            if (GetIdSuivi(libelle) == "00003")
            {
                cbxCommandeLivresLibelleSuivi.Enabled = false;
                btnReceptionCommandeLivresModifierSuivi.Enabled = false;
            }
            else
            {
                cbxCommandeLivresLibelleSuivi.Enabled = true;
                btnReceptionCommandeLivresModifierSuivi.Enabled = true;
            }
          
        }

        /// <summary>
        /// Tri sur les colonnes par ordre inverse de la chronologie
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void dgvCommandesLivre_ColumnHeaderMouseClick_1(object sender, DataGridViewCellMouseEventArgs e)
        {
            string titreColonne = dgvCommandesLivre.Columns[e.ColumnIndex].HeaderText;
            List<CommandeDocument> sortedList = new List<CommandeDocument>();
            switch (titreColonne)
            {
                case "Date de commande":
                    sortedList = lesCommandesDocument.OrderBy(o => o.DateCommande).Reverse().ToList();
                    break;
                case "Montant":
                    sortedList = lesCommandesDocument.OrderBy(o => o.Montant).ToList();
                    break;
                case "Nombre d'exemplaires":
                    sortedList = lesCommandesDocument.OrderBy(o => o.NbExemplaire).ToList();
                    break;
                case "Suivi":
                    sortedList = lesCommandesDocument.OrderBy(o => o.Libelle).ToList();
                    break;
            }
            RemplirCommandesLivresListe(sortedList);
        }

        /// <summary>
        /// Enregistrement d'une commande de livre dans la base de données
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnReceptionCommandeLivreValider_Click(object sender, EventArgs e)
        {
            if (!txbCommandeLivreNumero.Text.Equals("") && !txbCommandeLivreNbExemplaires.Text.Equals("") && !txbCommandeLivreMontant.Text.Equals(""))
            {
                string id = txbCommandeLivreNumero.Text;
                int nbExemplaire = int.Parse(txbCommandeLivreNbExemplaires.Text);
                double montant = double.Parse(txbCommandeLivreMontant.Text);
                DateTime dateCommande = dtpCommandeLivre.Value;
                string idLivreDvd = txbCommandesLivresNumRecherche.Text;
                string idSuivi = lesSuivis[0].Id;
                string libelle = lesSuivis[0].Libelle;

                Commande commande = new Commande(id, dateCommande, montant);

                if (controller.CreerCommande(commande))
                {
                    controller.CreerCommandeDocument(id, nbExemplaire, idLivreDvd, idSuivi);
                    MessageBox.Show("La commande "+ id + " a bien été enregistrée.", "Information");
                    AfficheReceptionCommandesLivre();
                }
                else
                {
                    MessageBox.Show("numéro de commande déjà existant", "Erreur");
                }
            }
            else
            {
                MessageBox.Show("Tout les champs sont obligatoires.", "Information");
            }
        }

        /// <summary>
        /// Modification de l'étape de suivi d'une commande de livre dans la base de données
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnModifierSuiviCommandeLivre_Click(object sender, EventArgs e)
        {
            string id = txbCommandeLivreNumero.Text;
            int nbExemplaire = int.Parse(txbCommandeLivreNbExemplaires.Text);
            double montant = double.Parse(txbCommandeLivreMontant.Text);
            DateTime dateCommande = dtpCommandeLivre.Value;
            string idLivreDvd = txbCommandesLivresNumRecherche.Text;
            string idSuivi = GetIdSuivi(cbxCommandeLivresLibelleSuivi.Text);

            try
            {
                string libelle = cbxCommandeLivresLibelleSuivi.SelectedItem.ToString();

                CommandeDocument commandedocument = new CommandeDocument(id, dateCommande, montant, nbExemplaire, idLivreDvd, idSuivi, libelle);
                if (MessageBox.Show("Voulez-vous modifier le suivi de la commande " + commandedocument.Id + " en " + libelle + " ?", "Confirmation", MessageBoxButtons.YesNo) == DialogResult.Yes)
                {
                    controller.ModifierSuiviCommandeDocument(commandedocument.Id, commandedocument.NbExemplaire, commandedocument.IdLivreDvd, commandedocument.IdSuivi);
                    MessageBox.Show("L'étape de suivi de la commande " + id + " a bien été modifiée.", "Information");
                    AfficheReceptionCommandesLivre();
                    cbxCommandeLivresLibelleSuivi.Items.Clear();
                }
            }
            catch (NullReferenceException)
            {
                MessageBox.Show("La nouvelle étape de suivi de la commande doit être sélectionnée.", "Information");
            }
        }
        
        /// <summary>
        /// Suppression d'une commande dans la base de données
        /// Si elle n'a pas encore été livrée 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnCommandeLivreSupprimer_Click(object sender, EventArgs e)
        {
            if (dgvCommandesLivre.SelectedRows.Count > 0)
            {
                CommandeDocument commandedocument = (CommandeDocument)bdgCommandesLivre.List[bdgCommandesLivre.Position];
                if (commandedocument.Libelle == "en cours" || commandedocument.Libelle == "relancée")
                {
                    if (MessageBox.Show("Voulez-vous vraiment supprimer la commande " + commandedocument.Id + " ?", "Confirmation de suppression", MessageBoxButtons.YesNo) == DialogResult.Yes)
                    {
                        controller.SupprimerCommandeDocument(commandedocument);
                        AfficheReceptionCommandesLivre();
                    }
                }
                else
                {
                    MessageBox.Show("La commande sélectionnée a été livrée, elle ne peut pas être supprimée.", "Information");
                }
            }
            else
            {
                MessageBox.Show("Une ligne doit être sélectionnée.", "Information");
            }
        }

        #endregion

        #region Onglet CommandesDvd
        private readonly BindingSource bdgCommandesDvd = new BindingSource();

        /// <summary>
        /// Ouverture de l'onglet Commandes de dvd :
        /// appel des méthodes pour remplir le datagrid des commandes de dvd
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void TabCommandesDvd_Enter(object sender, EventArgs e)
        {
            lesDvd = controller.GetAllDvd();
            lesSuivis = controller.GetAllSuivis();
            dtpCommandeDvd.Value = DateTime.Now;
            gbxInfosCommandeDvd.Enabled = false;
            gbxEtapeSuiviDvd.Enabled = false;
        }

        /// <summary>
        /// Masque la groupBox des suivis
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void GbxInfosCommandeDvd_Enter(object sender, EventArgs e)
        {
            gbxEtapeSuiviDvd.Enabled = false;
        }

        /// <summary>
        /// Affiche la groupBox des suivis
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BtnInfosCommandeDvdAnnuler_Click(object sender, EventArgs e)
        {
            gbxEtapeSuiviDvd.Enabled = true;
            gbxInfosCommandeDvd.Enabled = false;
        }

        /// <summary>
        /// Masque la groupBox des informations de commande et le numéro de recherche
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void GbxEtapeSuiviDvd_Enter(object sender, EventArgs e)
        {
            gbxInfosCommandeDvd.Enabled = false;
            txbCommandesDvdNumRecherche.Enabled = false;
        }

        /// <summary>
        /// Affiche la groupBox des commandes et le numéro de recherche
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BtnEtapeSuiviAnnulerDvd_Click(object sender, EventArgs e)
        {
            gbxEtapeSuiviDvd.Enabled = false;
            gbxInfosCommandeDvd.Enabled = true;
            txbCommandesDvdNumRecherche.Enabled = true;
        }


        /// <summary>
        /// Remplit la datagrid avec la liste reçue en paramètre
        /// </summary>
        /// <param name="lesCommandesDocument"></param>
        private void RemplirCommandesDvdListe(List<CommandeDocument> lesCommandesDocument)
        {
            if (lesCommandesDocument != null)
            {
                bdgCommandesDvd.DataSource = lesCommandesDocument;
                dgvCommandesDvd.DataSource = bdgCommandesDvd;
                dgvCommandesDvd.Columns["id"].Visible = false;
                dgvCommandesDvd.Columns["idLivreDvd"].Visible = false;
                dgvCommandesDvd.Columns["idSuivi"].Visible = false;
                dgvCommandesDvd.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.AllCells;
                dgvCommandesDvd.Columns["dateCommande"].DisplayIndex = 0;
                dgvCommandesDvd.Columns["montant"].DisplayIndex = 1;
                dgvCommandesDvd.Columns[5].HeaderCell.Value = "Date de commande";
                dgvCommandesDvd.Columns[0].HeaderCell.Value = "Nombre d'exemplaires";
                dgvCommandesDvd.Columns[3].HeaderCell.Value = "Suivi";
            }
            else
            {
                bdgCommandesDvd.DataSource = null;
            }
        }

        /// <summary>
        /// Mise à jour de la liste des commandes de dvd
        /// </summary>
        private void AfficheReceptionCommandesDvd()
        {
            string idDocument = txbCommandesDvdNumRecherche.Text;
            lesCommandesDocument = controller.GetCommandesDocument(idDocument);
            RemplirCommandesDvdListe(lesCommandesDocument);
        }

        /// <summary>
        /// Recherche les commandes concernant le dvd
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnCommandesDvdNumRecherche_Click(object sender, EventArgs e)
        {
            if (!txbCommandesDvdNumRecherche.Text.Equals(""))
            {
                Dvd dvd = lesDvd.Find(x => x.Id.Equals(txbCommandesDvdNumRecherche.Text));
                if (dvd != null)
                {
                    AfficheReceptionCommandesDvd();
                    gbxInfosCommandeDvd.Enabled = true;
                    AfficheReceptionCommandesDvdInfos(dvd);
                }
                else
                {
                    MessageBox.Show("numéro introuvable");
                }
            }
            else
            {
                MessageBox.Show("Le numéro de document est obligatoire", "Information");
            }
        }

        /// <summary>
        /// Affichage des informations du dvd
        /// </summary>
        /// <param name="dvd"></param>
        private void AfficheReceptionCommandesDvdInfos(Dvd dvd)
        {
            txbCommandeDvdTitre.Text = dvd.Titre;
            txbCommandeDvdRealisateur.Text = dvd.Realisateur;
            txbCommandeDvdSynopsis.Text = dvd.Synopsis;
            txbCommandeDvdDuree.Text = dvd.Duree.ToString();
            txbCommandeDvdGenre.Text = dvd.Genre;
            txbCommandeDvdPublic.Text = dvd.Public;
            txbCommandeDvdRayon.Text = dvd.Rayon;
            txbCommandeDvdCheminImage.Text = dvd.Image;
            string image = dvd.Image;
            try
            {
                pictureBox2.Image = Image.FromFile(image);
            }
            catch
            {
                pictureBox2.Image = null;
            }
            AfficheReceptionCommandesDvd();
        }

        /// <summary>
        /// Selon le libelle dans la txbBox, affichage des étapes de suivi correspondantes
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void lblSuiviEtapeDvd_TextChanged(object sender, EventArgs e)
        {
            string etapeSuivi = lblEtapeSuiviDvd.Text;
            RemplirCbxCommandeDvdLibelleSuivi(etapeSuivi);
        }

        /// <summary>
        /// Remplissage de la liste de suivi des commandes de dvd
        /// </summary>
        /// <param name="etapeSuivi"></param>
        private void RemplirCbxCommandeDvdLibelleSuivi(string etapeSuivi)
        {
            cbxCommandeDvdLibelleSuivi.Items.Clear();
            if (etapeSuivi == "livrée")
            {
                cbxCommandeDvdLibelleSuivi.Text = "";
                cbxCommandeDvdLibelleSuivi.Items.Add("réglée");
            }
            else if (etapeSuivi == "en cours")
            {
                cbxCommandeDvdLibelleSuivi.Text = "";
                cbxCommandeDvdLibelleSuivi.Items.Add("relancée");
                cbxCommandeDvdLibelleSuivi.Items.Add("livrée");
            }
            else if (etapeSuivi == "relancée")
            {
                cbxCommandeDvdLibelleSuivi.Text = "";
                cbxCommandeDvdLibelleSuivi.Items.Add("en cours");
                cbxCommandeDvdLibelleSuivi.Items.Add("livrée");
            }
        }

        /// <summary>
        /// Affiche les informations de la commande sélectionnée 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void dgvCommandeDvd_RowEnter(object sender, DataGridViewCellEventArgs e)
        {
            DataGridViewRow row = dgvCommandesDvd.Rows[e.RowIndex];

            string id = row.Cells["Id"].Value.ToString();
            DateTime dateCommande = (DateTime)row.Cells["dateCommande"].Value;
            double montant = double.Parse(row.Cells["Montant"].Value.ToString());
            int nbExemplaire = int.Parse(row.Cells["NbExemplaire"].Value.ToString());
            string libelle = row.Cells["Libelle"].Value.ToString();

            txbCommandeDvdNumero.Text = id;
            txbCommandeDvdNbExemplaires.Text = nbExemplaire.ToString();
            txbCommandeDvdMontant.Text = montant.ToString();
            dtpCommandeDvd.Value = dateCommande;

            lblEtapeSuiviDvd.Text = libelle;
            if (GetIdSuivi(libelle) == "00003")
            {
                cbxCommandeDvdLibelleSuivi.Enabled = false;
                btnReceptionCommandeDvdModifierSuivi.Enabled = false;
            }
            else
            {
                cbxCommandeDvdLibelleSuivi.Enabled = true;
                btnReceptionCommandeDvdModifierSuivi.Enabled = true;
            }
        }

        /// <summary>
        /// Tri sur les colonnes par ordre inverse de la chronologie
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void dgvCommandesDvd_ColumnHeaderMouseClick_1(object sender, DataGridViewCellMouseEventArgs e)
        {
            string titreColonne = dgvCommandesDvd.Columns[e.ColumnIndex].HeaderText;
            List<CommandeDocument> sortedList = new List<CommandeDocument>();
            switch (titreColonne)
            {
                case "Date de commande":
                    sortedList = lesCommandesDocument.OrderBy(o => o.DateCommande).Reverse().ToList();
                    break;
                case "Montant":
                    sortedList = lesCommandesDocument.OrderBy(o => o.Montant).ToList();
                    break;
                case "Nombre d'exemplaires":
                    sortedList = lesCommandesDocument.OrderBy(o => o.NbExemplaire).ToList();
                    break;
                case "Suivi":
                    sortedList = lesCommandesDocument.OrderBy(o => o.Libelle).ToList();
                    break;
            }
            RemplirCommandesDvdListe(sortedList);
        }

        /// <summary>
        /// Enregistrement d'une nouvelle commande de dvd
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnReceptionCommandeDvdValider_Click(object sender, EventArgs e)
        {
            if (!txbCommandeDvdNumero.Text.Equals("") && !txbCommandeDvdNbExemplaires.Text.Equals("") && !txbCommandeDvdMontant.Text.Equals(""))
            {
                string idLivreDvd = txbCommandesDvdNumRecherche.Text;
                string idSuivi = lesSuivis[0].Id;
                string libelle = lesSuivis[0].Libelle;
                string id = txbCommandeDvdNumero.Text;
                int nbExemplaire = int.Parse(txbCommandeDvdNbExemplaires.Text);
                double montant = double.Parse(txbCommandeDvdMontant.Text);
                DateTime dateCommande = dtpCommandeDvd.Value;

                Commande commande = new Commande(id, dateCommande, montant);
                CommandeDocument commandedocument = new CommandeDocument(id, dateCommande, montant, nbExemplaire, idLivreDvd, idSuivi, libelle);
                if (controller.CreerCommande(commande))
                {
                    controller.CreerCommandeDocument(commandedocument.Id, commandedocument.NbExemplaire, commandedocument.IdLivreDvd, commandedocument.IdSuivi);
                    MessageBox.Show("La commande " + id + " a bien été enregistrée.", "Information");
                    AfficheReceptionCommandesDvd();
                }
                else
                {
                    MessageBox.Show("numéro de commande déjà existant", "Erreur");
                }
            }
            else
            {
                MessageBox.Show("Toutes les informations de commande doivent être saisies.", "Information");
            }
        }

        /// <summary>
        /// Modification de l'étape de suivi d'une commande de dvd
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnReceptionCommandeDvdModifierSuivi_Click(object sender, EventArgs e)
        {
            try
            {
                string idLivreDvd = txbCommandesDvdNumRecherche.Text;
                string idSuivi = GetIdSuivi(cbxCommandeDvdLibelleSuivi.Text);
                string libelle = cbxCommandeDvdLibelleSuivi.SelectedItem.ToString();
                string id = txbCommandeDvdNumero.Text;
                int nbExemplaire = int.Parse(txbCommandeDvdNbExemplaires.Text);
                double montant = double.Parse(txbCommandeDvdMontant.Text);
                DateTime dateCommande = dtpCommandeDvd.Value;

                Commande commande = new Commande(id, dateCommande, montant);
                CommandeDocument commandedocument = new CommandeDocument(id, dateCommande, montant, nbExemplaire, idLivreDvd, idSuivi, libelle);
                if (MessageBox.Show("Voulez-vous modifier le suivi de la commande " + commandedocument.Id + " en " + libelle + " ?", "Confirmation", MessageBoxButtons.YesNo) == DialogResult.Yes)
                {
                    controller.ModifierSuiviCommandeDocument(commandedocument.Id, commandedocument.NbExemplaire, commandedocument.IdLivreDvd, commandedocument.IdSuivi);
                    AfficheReceptionCommandesDvd();
                    cbxCommandeDvdLibelleSuivi.Items.Clear();
                }
            }
            catch (NullReferenceException)
            {
                MessageBox.Show("La nouvelle étape de suivi de la commande doit être sélectionnée.", "Information");
            }
        }

        /// <summary>
        /// Suppression d'une commande de dvd si elle n'a pas encore été livrée
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnCommandeDvdSupprimer_Click(object sender, EventArgs e)
        {
            if (dgvCommandesDvd.SelectedRows.Count > 0)
            {
                CommandeDocument commandeDocument = (CommandeDocument)bdgCommandesDvd.List[bdgCommandesDvd.Position];
                if (commandeDocument.Libelle == "en cours" || commandeDocument.Libelle == "relancée")
                {
                    if (MessageBox.Show("Voulez-vous vraiment supprimer la commande " + commandeDocument.Id + " ?", "Confirmation de suppression", MessageBoxButtons.YesNo) == DialogResult.Yes)
                    {
                        controller.SupprimerCommandeDocument(commandeDocument);
                        AfficheReceptionCommandesDvd();
                    }
                }
                else
                {
                    MessageBox.Show("La commande sélectionnée a été livrée elle ne peut pas être supprimée.", "Information");
                }
            }
            else
            {
                MessageBox.Show("Une ligne doit être sélectionnée.", "Information");
            }
        }


        #endregion

        #region Onglet CommandesRevues

        private readonly BindingSource bdgAbonnementsRevue = new BindingSource();
        private List<Abonnement> lesAbonnementsRevue = new List<Abonnement>();

        /// <summary>
        /// Ouverture de l'onglet Commandes de revues :
        /// appel des méthodes pour remplir le datagrid des abonnements d'une revue
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void TabCommandesRevues_Enter(object sender, EventArgs e)
        {
            lesRevues = controller.GetAllRevues();
            gbxInfosCommandeRevue.Enabled = false;
            dtpCommandeDvd.Value = DateTime.Now;
        }

        /// <summary>
        /// Remplit la datagrid avec la liste reçue en paramètre
        /// </summary>
        /// <param name="lesAbonnements"></param>
        private void RemplirAbonnementsRevueListe(List<Abonnement> lesAbonnementsRevue)
        {
            if (lesAbonnementsRevue != null)
            {
                bdgAbonnementsRevue.DataSource = lesAbonnementsRevue;
                dgvAbonnementsRevue.DataSource = bdgAbonnementsRevue;
                dgvAbonnementsRevue.Columns["id"].Visible = false;
                dgvAbonnementsRevue.Columns["idRevue"].Visible = false;
                dgvAbonnementsRevue.Columns["titre"].Visible = false;
                dgvAbonnementsRevue.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.AllCells;
                dgvAbonnementsRevue.Columns["dateCommande"].DisplayIndex = 0;
                dgvAbonnementsRevue.Columns["montant"].DisplayIndex = 1;
                dgvAbonnementsRevue.Columns[4].HeaderCell.Value = "Date de commande";
                dgvAbonnementsRevue.Columns[0].HeaderCell.Value = "Date de fin d'abonnement";
            }
            else
            {
                bdgAbonnementsRevue.DataSource = null;
            }
        }

        /// <summary>
        /// Affiche la liste des abonnements d'une revue
        /// </summary>
        private void AfficheReceptionAbonnementsRevue()
        {
            string idDocument = txbCommandesRevueNumRecherche.Text;
            lesAbonnementsRevue = controller.GetAbonnementRevue(idDocument);
            RemplirAbonnementsRevueListe(lesAbonnementsRevue);
        }

        /// <summary>
        /// Recherche d'une revue
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnCommandesRevueNumRecherche_Click(object sender, EventArgs e)
        {
            if (!txbCommandesRevueNumRecherche.Text.Equals(""))
            {
                Revue revue = lesRevues.Find(x => x.Id.Equals(txbCommandesRevueNumRecherche.Text));
                if (revue != null)
                {
                    AfficheReceptionAbonnementsRevue();
                    gbxInfosCommandeRevue.Enabled = true;
                    AfficheReceptionAbonnementsRevueInfos(revue);
                }
                else
                {
                    MessageBox.Show("Ce numéro de revue n'existe pas.");
                }
            }
            else
            {
                MessageBox.Show("Le numéro de revue est obligatoire.");
            }
        }

        /// <summary>
        /// Affichage des informations d'une revue
        /// </summary>
        /// <param name="revue"></param>
        private void AfficheReceptionAbonnementsRevueInfos(Revue revue)
        {
            txbCommandeRevueTitre.Text = revue.Titre;
            txbCommandeRevuePeriodicite.Text = revue.Periodicite;
            txbCommandeRevueDelai.Text = revue.DelaiMiseADispo.ToString();
            txbCommandeRevueGenre.Text = revue.Genre;
            txbCommandeRevuePublic.Text = revue.Public;
            txbCommandeRevueRayon.Text = revue.Rayon;
            txbCommandeRevueCheminImage.Text = revue.Image; 
            string image = revue.Image;
            try
            {
                pictureBox4.Image = Image.FromFile(image);
            }
            catch
            {
                pictureBox4.Image = null;
            }
            AfficheReceptionAbonnementsRevue();
        }

        /// <summary>
        /// Affichage des informations de l'abonnement sélectionné
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void dgvAbonnementsRevue_RowEnter(object sender, DataGridViewCellEventArgs e)
        {
            DataGridViewRow row = dgvAbonnementsRevue.Rows[e.RowIndex];
            string id = row.Cells["Id"].Value.ToString();
            DateTime dateCommande = (DateTime)row.Cells["dateCommande"].Value;
            double montant = double.Parse(row.Cells["Montant"].Value.ToString());
            DateTime dateFinAbonnement = (DateTime)row.Cells["DateFinAbonnement"].Value;

            txbCommandeRevueNumero.Text = id;
            txbCommandeRevueMontant.Text = montant.ToString();
            dtpCommandeRevue.Value = dateCommande;
            dtpCommandeRevueAbonnementFin.Value = dateFinAbonnement;  
        }

        /// <summary>
        /// Tri sur les colonnes par ordre inverse de la chronologie
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void dgvAbonnementsRevue_ColumnHeaderMouseClick(object sender, DataGridViewCellMouseEventArgs e)
        {
            string titreColonne = dgvAbonnementsRevue.Columns[e.ColumnIndex].HeaderText;
            List<Abonnement> sortedList = new List<Abonnement>();
            switch (titreColonne)
            {
                case "Date de commande":
                    sortedList = lesAbonnementsRevue.OrderBy(o => o.DateCommande).Reverse().ToList();
                    break;
                case "Montant":
                    sortedList = lesAbonnementsRevue.OrderBy(o => o.Montant).ToList();
                    break;
                case "Date de fin d'abonnement":
                    sortedList = lesAbonnementsRevue.OrderBy(o => o.DateFinAbonnement).Reverse().ToList();
                    break;
            }
            RemplirAbonnementsRevueListe(sortedList);
        }

        /// <summary>
        /// Enregistrement d'un abonnement de revue dans la base de données
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnReceptionAbonnementRevueValider_Click(object sender, EventArgs e)
        {
            if (!txbCommandeRevueNumero.Text.Equals("") && !txbCommandeRevueMontant.Text.Equals(""))
            {
                string idRevue = txbCommandesRevueNumRecherche.Text;
                string titre = txbCommandeRevueTitre.Text;
                string id = txbCommandeRevueNumero.Text;
                double montant = double.Parse(txbCommandeRevueMontant.Text);
                DateTime dateCommande = dtpCommandeRevue.Value;
                DateTime dateFinAbonnement = dtpCommandeRevueAbonnementFin.Value;
                
                Commande commande = new Commande(id, dateCommande, montant);
                Abonnement abonnement = new Abonnement(id, dateCommande, montant, dateFinAbonnement, idRevue, titre);

                if (controller.CreerCommande(commande))
                {
                    controller.CreerAbonnementRevue(id, dateFinAbonnement, idRevue);
                    MessageBox.Show("La commande " + id + " a bien été enregistrée.", "Information");
                    AfficheReceptionAbonnementsRevue();
                }
                else
                {
                    MessageBox.Show("numéro de commande déjà existant", "Erreur");
                }
            }
            else
            {
                MessageBox.Show("Tous les champs sont obligatoires", "Information");
            }
        }

        /// <summary>
        /// Retourne vrai si la date de parution est entre les 2 autres dates
        /// </summary>
        /// <param name="dateCommande"></param>
        /// <param name="dateFinAbonnement"></param>
        /// <param name="dateParution"></param>
        /// <returns></returns>
        public bool ParutionDansAbonnement(DateTime dateCommande, DateTime dateFinAbonnement, DateTime dateParution)
        {
            return (DateTime.Compare(dateCommande, dateParution) < 0 && DateTime.Compare(dateParution, dateFinAbonnement) < 0);
        }

        /// <summary>
        /// Vérifie si aucun exemplaire n'est rattaché à un abonnement de revue
        /// </summary>
        /// <param name="abonnement"></param>
        /// <returns></returns>
        public bool VerificationExemplaire(Abonnement abonnement)
        {
            List<Exemplaire> lesExemplaires = controller.GetExemplairesRevue(abonnement.IdRevue);
            bool datedeparution = false;
            foreach (Exemplaire exemplaire in lesExemplaires.Where(exemplaires => ParutionDansAbonnement(abonnement.DateCommande, abonnement.DateFinAbonnement, exemplaires.DateAchat)))
            {
                datedeparution = true;

            }
            return !datedeparution;
        }

        /// <summary>
        /// Suppression d'un abonnement de revue dans la base de données
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnAbonnementRevueSupprimer_Click(object sender, EventArgs e)
        {
            if (dgvAbonnementsRevue.SelectedRows.Count > 0)
            {
                Abonnement abonnement = (Abonnement)bdgAbonnementsRevue.Current;
                if (MessageBox.Show("Souhaitez-vous confirmer la suppression de l'abonnement " + abonnement.Id + " ?", "Confirmation de la suppression", MessageBoxButtons.OKCancel) == DialogResult.OK)
                {

                    if (VerificationExemplaire(abonnement))
                    {
                        if (controller.SupprimerAbonnementRevue(abonnement))
                        {
                            AfficheReceptionAbonnementsRevue();
                        }
                        else
                        {
                            MessageBox.Show("Une erreur s'est produite.", "Erreur");
                        }
                    }
                    else
                    {
                        MessageBox.Show("Cet abonnement contient un ou plusieurs exemplaires, il ne peut donc pas être supprimé.", "Information");
                    }
                }
            }
            else
            {
                MessageBox.Show("Une ligne doit être sélectionnée.", "Information");
            }
        }


        #endregion
    }
}
