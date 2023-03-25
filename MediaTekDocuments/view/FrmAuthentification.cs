using System;
using MediaTekDocuments.controller;
using MediaTekDocuments.model;
using System.Windows.Forms;

namespace MediaTekDocuments.view
{
    public partial class FrmAuthentification : Form
    {
        private readonly FrmAuthentificationController controller;

        /// <summary>
        /// Constructeur : création du contrôleur lié à ce formulaire
        /// </summary>
        public FrmAuthentification()
        {
            InitializeComponent();
            this.controller = new FrmAuthentificationController();
        }

        /// <summary>
        /// Connexion à l'application grâce aux identifiants saisis
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnConnexion_Click(object sender, EventArgs e)
        {
            string login = txbLogin.Text;
            string password = txbPassword.Text;

            if (!txbLogin.Text.Equals("") && !txbPassword.Text.Equals(""))
            {
                Service service = controller.GetUtilisateur(login, password);

                if (service == null)
                {
                    MessageBox.Show("Erreur d'authentification", "Alerte");
                    txbPassword.Text = "";
                }
                else if(service.Libelle == "culture")
                {
                    MessageBox.Show("Vous n'avez pas les droits d'accès à l'application.", "Alerte");
                    Application.Exit();
                }
                else
                {
                    MessageBox.Show("Vous êtes connecté", "Information");
                    FrmMediatek frmMediatek = new FrmMediatek(service);
                    frmMediatek.ShowDialog();
                }
            }
            else
            {
                MessageBox.Show("Tous les champs sont obligatoires");
            }
        }
    }
}
