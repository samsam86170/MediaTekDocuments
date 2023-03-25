Feature: SpecFlowMediatekDocuments
	Recherche d'un livre

@rechercheLivreNumero
Scenario: Chercher un livre avec son numero dans txbLivresNumRecherche
	Given Je saisis la valeur 00003 dans txbLivresNumRecherche
	When Je clic sur le bouton Rechercher
	Then Les informations détaillées affichent le titre 'Et je danse'

Scenario: Chercher une liste de livres avec le genre dans cbxLivresGenres
	Given Je sélectionne la valeur 'Policier' dans cbxLivresGenres
	Then Le résultat est 8 livres dans dgvLivresListe

Scenario: Chercher une liste de livres avec le public dans cbxLivresPublics
	Given Je sélectionne la valeur 'Adultes' dans cbxLivresPublics
	Then Le résultat est 12 livres dans dgvLivresListe

Scenario: Chercher une liste de livres avec le rayon dans cbxLivresRayons
	Given Je sélectionne la valeur 'Sciences' dans cbxLivresRayons
	Then Le résultat est 0 livre

Scenario: Chercher un livre avec son titre dans txbLivresTitreRecherche
	Given Je saisis la valeur 'Et je danse' dans txbLivresTitreRecherche
	When Je clic sur le bouton Rechercher
	Then Les informations détaillées affichent le titre 'Et je danse'