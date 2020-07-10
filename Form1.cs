using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml.Serialization;

namespace LeChaudron
{
    public partial class ChaudronMaster : Form
    {
        Laboratoire lab = new Laboratoire(0);
        void MajRecettes()
        {
            List<string> ingr = lab.recettes.Keys.ToList();
            foreach (string st in ingr)
            {
                if (!RecettesLB.Items.Contains(st))
                    RecettesLB.Items.Add(st);
            }
            if (RecettesLB.SelectedIndex > -1)
            {
                lSRecette.Text = lab.recettes[RecettesLB.SelectedItem.ToString()].Afficher_Long();
                tBRAV.Text = RecettesLB.SelectedItem.ToString();
                bValiderMission.Enabled = lab.missionActuelle.verifierConformite(lab.recettes[RecettesLB.SelectedItem.ToString()]);
            }
                
        }
        void MajIndicMission()
        {
            bool missValid = lab.missionActuelle.verifierConformite(lab.recetteCourante);
            if (missValid)
            {
                indicateurMission.Text = "Peut valider Mission !";
                indicateurMission.ForeColor = Color.Gold;
            }
            else
            {
                indicateurMission.Text = "Non attendu par la Mission...";
                indicateurMission.ForeColor = Color.White;
            }
        }
        void MajIngredients()
        {
            List<string> ingr = lab.GetNomsIngredients();
            foreach (string st in ingr)
            {
                if (!IngredientsLB.Items.Contains(st))
                    IngredientsLB.Items.Add(st);
            }
            if (IngredientsLB.SelectedIndex> -1)
                lSIngredient.Text = lab.ingredients[IngredientsLB.SelectedItem.ToString()].Afficher();
            MajPrincipes();
        }
        void MajPrincipes()
        {
            List<string> ingr = lab.composesConnus;
            foreach (string st in ingr)
            {
                if (!PrincipesLB.Items.Contains(st))
                    PrincipesLB.Items.Add(st);
            }
            if (PrincipesLB.SelectedIndex > -1)
                lSPrincipe.Text = lab.composes[PrincipesLB.SelectedItem.ToString()].Afficher();
        }
        void MajPrestigeEtCredits()
        {
            lPrestige.Text = "Prestige : " + lab.Prestige;
            lCredits.Text = "Credits : " + lab.Fonds;
            progressXP.Maximum = lab.Prestige + 1;
            progressXP.Value = lab.Xp;
        }
        void MajEprouvette()
        {
            lEprouvette.Text = lab.recetteCourante.Afficher();
            int ph = lab.recetteCourante.ProduitFini.CalculerPH();
            labAcid.Text = "Acidite : " + ph;
            ProgressAcide.Value = Math.Min(Math.Max(0, ph), 40);
            int temp = lab.recetteCourante.Temperature;
            labTemp.Text = "Temperature : " + temp;
            ProgressTemp.Value = Math.Min(Math.Max(0, temp), 40);
            labelEffets.Text = lab.recetteCourante.ProduitFini.AfficherEffets();
            ContenuLB.Items.Clear();
            for (int i = 0; i < lab.recetteCourante.ProduitFini.Composition.Count; i++)
            {
                ContenuLB.Items.Add(lab.recetteCourante.ProduitFini.Composition[i].Nom);
            }
            MajIndicMission();
        }
        void MajTemperatures()
        {
            lregtemp.Text = "Régler la Température à " + trackBarTemp.Value + " Degrés";
            lregevap.Text = "Evaporer à " + trackBarEvap.Value + " Degrés";
            lregdist.Text = "Distiller à " + trackBarDist.Value + " Degrés";
        }
        void MajEtape()
        {

            if (lab.etape == 0)
            {
                lEtape.Text = "Etape : Contrôle";
                lEtape.ForeColor = Color.Lime;
                chBGardPrecip.Enabled = false;
                chBGardPrecip.Checked = false;
                bAjIng.Enabled = true;
                bAjRec.Enabled = true;
                bGarder.Enabled = true;
                bev.Enabled = true;
                breg.Enabled = true;
                bdist.Enabled = true;
            }
            else if (lab.etape == 1)
            {
                lEtape.Text = "Etape : Destructions";
                lEtape.ForeColor = Color.Yellow;
                chBGardPrecip.Enabled = false;
                bAjIng.Enabled = false;
                bAjRec.Enabled = false;
                bGarder.Enabled = false;
                bev.Enabled = false;
                breg.Enabled = false;
                bdist.Enabled = false;
            }
            else if (lab.etape == 2)
            {
                lEtape.Text = "Etape : Transformations";
                lEtape.ForeColor = Color.LightBlue;

            }
            else if (lab.etape == 3)
            {
                MajPrincipes();
                lEtape.Text = "Etape : Precipitations";
                lEtape.ForeColor = Color.White;
                chBGardPrecip.Enabled = true;
            }
            MajInfos();
        }
        void MajInfos()
        {
            if (lab.recetteCourante.ProduitFini.Precipite)
            {
                linfo1.Text = "Poudre : Prix de vente X 2,\nPas de réactions, distillations, évaporations\nMelanges entre poudres seulement";
                linfo1.ForeColor = Color.Gold;
                bev.Enabled = false;
                bdist.Enabled = false;
                bAjIng.Enabled = false;
            }
            else
            {
                linfo1.Text = "Liquide : Prix de vente X 1,\nMelanges entre liquides seulement";
                linfo1.ForeColor = Color.White;
                if (lab.etape == 0 )
                {
                    bev.Enabled = true;
                    bdist.Enabled = true;
                    if (IngredientsLB.SelectedIndex > -1 && lab.Prestige < lab.ingredients[IngredientsLB.SelectedItem.ToString()].Prestige)
                    {
                        bAjIng.Enabled = false;
                    }
                    else
                    {
                        bAjIng.Enabled = true;
                    }
                }
            }
            if (RecettesLB.SelectedIndex > -1)
            {
                if (lab.recetteCourante.ProduitFini.Precipite != lab.recettes[RecettesLB.SelectedItem.ToString()].ProduitFini.Precipite)
                {
                    bAjRec.Enabled = false;
                }
                else
                {
                    bAjRec.Enabled = true;
                }
            }
                
        }
        void Maj()
        {
            IngredientsLB.Items.Clear();
            RecettesLB.Items.Clear();
            PrincipesLB.Items.Clear();
            MajEtape();
            MajIngredients();
            MajPrincipes();
            MajRecettes();
            MajMission();
            MajEprouvette();
            MajTemperatures();
            MajInfos();
            bValiderMission.Enabled = false;
        }
        void MajMission()
        {
            lMission.Text = lab.missionActuelle.Afficher();
            MajPrestigeEtCredits();
            MajIndicMission();
        }
        void DoubleBuffers()
        {
            SetDoubleBuffered(this.tableLayoutPanel1);
            SetDoubleBuffered(this.tableLayoutPanel2);
            SetDoubleBuffered(this.tableLayoutPanel3);
            SetDoubleBuffered(this.tableLayoutPanel4);
            SetDoubleBuffered(this.tableLayoutPanel5);
            SetDoubleBuffered(this.tableLayoutPanel6);
            SetDoubleBuffered(this.tableLayoutPanel7);
            SetDoubleBuffered(this.tableLayoutPanel8);
            SetDoubleBuffered(this.tableLayoutPanel9);
            SetDoubleBuffered(this.tableLayoutPanel10);
            SetDoubleBuffered(this.tableLayoutPanel11);
            SetDoubleBuffered(this.tableLayoutPanel12);
            SetDoubleBuffered(this.tableLayoutPanel13);
            SetDoubleBuffered(this.tableLayoutPanel14);
            SetDoubleBuffered(this.tableLayoutPanel15);
            SetDoubleBuffered(this.tableLayoutPanel16);
            SetDoubleBuffered(this.tableLayoutPanel17);
            SetDoubleBuffered(this.tableLayoutPanel18);
            SetDoubleBuffered(this.tableLayoutPanel19);
            SetDoubleBuffered(this.tableLayoutPanel20);
            SetDoubleBuffered(this.tableLayoutPanel21);
            SetDoubleBuffered(this.tableLayoutPanel22);
            SetDoubleBuffered(this.tableLayoutPanel23);
            SetDoubleBuffered(this.tableLayoutPanel24);
            SetDoubleBuffered(this.tableLayoutPanel25);

            SetDoubleBuffered(this.tableLayoutPanel1);
            SetDoubleBuffered(this.tableLayoutPanel1);
            SetDoubleBuffered(this.tableLayoutPanel1);
        }
        public ChaudronMaster()
        {
            InitializeComponent();
            DoubleBuffers();
            Maj();
        }

        public class GenerateurDeNom
        {
            private static List<String> Fonctions = new List<string>() {"Brillant","Dorure","Volatile","Résidu","Vent","Sulfureux","Thé","Distillat","Macéré",
                "Fumée","Glaire","Poudre","Concentré","Houille","Substance","Lueur","Suffocant","Pâte","Cristal","Coulant","Suintant","Baveux","Raclure",
                "Merveille","Miracle","Beauté","Crasse","Suif","Collant","Roche","Pierre","Petrole","Vif","Gris","Bleu","Vert","Atroce","Véritable","Lumière","Gras",
            "Rêche","Soyeux","Riche","Léger","Masse","Vapeur","Volonté","Séduction","Morsure","Etranglement","Mort","Vie","Peur","Joie","Teinture","Tourbe","Chiure"};
            public static string genererNomPropre(ref Random r)
            {
                int lenght = r.Next(2, 10);
                string[] consonants = { "b", "c", "d", "f", "g", "h", "j", "k", "l", "m", "l", "n", "p", "q", "r", "s", "sh", "zh", "t", "v", "w", "x", "ph", "th" };
                string[] vowels = { "a", "e", "i", "o", "u", "ae", "y", "é" };
                string Name = "";
                string first = consonants[r.Next(consonants.Length)];
                if(first.Length == 1)
                {
                    Name += first.ToUpper();
                }
                else
                {
                    Name += first[0].ToString().ToUpper() + first[1];
                }
                Name += vowels[r.Next(vowels.Length)];
                int b = 2;
                while (b < lenght)
                {
                    Name += consonants[r.Next(consonants.Length)];
                    b++;
                    Name += vowels[r.Next(vowels.Length)];
                    b++;
                }
                return Name;
            }
            public static string genererNom(ref Random r)
            {
                string retour = "";
                int N = r.Next(2, 5);
                for(int i=0;i<N;i++)
                {
                    if(r.Next(0,3)==0)
                    { 
                        retour += genererNomPropre(ref r);
                    }
                    else
                    {
                        retour += Fonctions[r.Next(0, Fonctions.Count)];
                    }
                    if(i != N-1)
                    {
                        if (r.Next(0, 3) == 0)
                        {
                            if (r.Next(0, 3) == 0)
                            {
                                if (r.Next(0, 3) == 0)
                                {
                                    retour += " sous ";
                                }
                                else
                                {
                                    retour += " sans ";
                                }
                            }
                            else
                            {
                                if (r.Next(0, 3) == 0)
                                {
                                    retour += " et ";
                                }
                                else
                                {
                                    retour += " dans ";
                                }
                            }
                        }
                        else
                        {
                            if (r.Next(0, 3) == 0)
                            {
                                retour += " pour ";
                            }
                            else
                            {
                                retour += " de ";
                            }
                        }
                    }
                }
                return retour;
            }
        }

        public enum Effets
        {
            Explosif,
            Stabilisateur,
            Soin,
            Poison,
            Parfum,
            Pestilence,
            Savoureux,
            Immonde

        }
        public class Compose
        {
            public string Nom;
            public int Complexite;
            //Stabilite
            public int TempMax;
            public int PhMin;
            public int PhMax;
            public List<string> Destructeurs;
            //Isoler
            public int Tevap;
            public List<string> Precipiteurs;
            //Influencer
            public int deltaPH;
            //Transformer
            public int trPhMin;
            public int trPhMax;
            public int trTempMin;
            public int trTempMax;
            public List<string> Catalyseurs;
            public string Resultat;
            //Effet
            public 
            List<Effets> Effet;

            public override bool Equals(object obj)
            {
                var compose = obj as Compose;
                return compose != null &&
                        Nom == compose.Nom;
            }
            public static Compose FromString(string nom)
            {
                Compose retour = new Compose();
                retour.Nom = nom;
                return retour;
            }
            public int getPrestige()
            {
                return Effet.Count() * Complexite;
            }
            public static Compose GenererAleatoire(int Complexite, ref Random r, ref List<string> NomsUtilises)
            {
                int tempSup = 40;
                int phSup = 40;
                Compose retour = new Compose();
                retour.Complexite = Complexite;
                string nom = GenerateurDeNom.genererNom(ref r);
                while(NomsUtilises.Contains(nom))
                {
                    nom = GenerateurDeNom.genererNom(ref r);
                }
                retour.Nom = nom;
                NomsUtilises.Add(nom);
                retour.Destructeurs = new List<string>();
                retour.Precipiteurs = new List<string>();
                retour.deltaPH = r.Next(-phSup/3, phSup/3 + 1);
                retour.trTempMax = r.Next(1, tempSup+1);
                retour.trTempMin = r.Next(1, retour.trTempMax+1);
                retour.trPhMin = r.Next(1, phSup+1);
                retour.trPhMax = r.Next(retour.trPhMin, phSup + 1);
                retour.TempMax = r.Next(retour.trTempMax, tempSup + 1);
                retour.Tevap = r.Next(1, retour.TempMax + 1);
                retour.PhMax = r.Next(retour.trPhMax, phSup + 1);
                retour.PhMin = r.Next(1, retour.trPhMin + 1);
                retour.Catalyseurs = new List<string>();
                retour.Effet = new List<Effets>();
                List<Effets> EffetsDispos = ((Effets[])Enum.GetValues(typeof(Effets))).ToList<Effets>();
                int nbEffets = Math.Min(Complexite,r.Next(0, EffetsDispos.Count / 2 + 1));
                for(int i=0;i<nbEffets;i++)
                {
                    Effets choisi = EffetsDispos[r.Next(0, EffetsDispos.Count)];
                    if (choisi == Effets.Explosif || choisi == Effets.Stabilisateur)
                    {
                        EffetsDispos.Remove(Effets.Explosif);
                        EffetsDispos.Remove(Effets.Stabilisateur);
                    }
                    if (choisi == Effets.Soin || choisi == Effets.Poison)
                    {
                        EffetsDispos.Remove(Effets.Soin);
                        EffetsDispos.Remove(Effets.Poison);
                    }
                    if (choisi == Effets.Parfum || choisi == Effets.Pestilence)
                    {
                        EffetsDispos.Remove(Effets.Parfum);
                        EffetsDispos.Remove(Effets.Pestilence);
                    }
                    if (choisi == Effets.Savoureux || choisi == Effets.Immonde)
                    {
                        EffetsDispos.Remove(Effets.Savoureux);
                        EffetsDispos.Remove(Effets.Immonde);
                    }
                    retour.Effet.Add(choisi);
                }
                return retour;
            }
            public string Afficher()
            {
                string retour = Nom +" ( Complexité "+Complexite+" )"+ " :" + '\n';
                retour += "    Stabilite : Température <" + TempMax + " Degres, Acidité " + PhMin + " - " + PhMax + '\n';
                string ch = "";
                if(deltaPH>0)
                {
                    ch = "+";
                }
                retour += "    Acidité :"+ch+deltaPH+'\n';
                retour += "    Temperature d'évaporation :" + Tevap+" Degres" + '\n';
                if(Effet.Count!=0)
                    retour += "    Effets : ";
                for (int i = 0; i < Effet.Count; i++)
                {
                    retour += Effet[i];
                    if(i==Effet.Count-1)
                    {
                        retour += '\n';
                    }
                    else
                    {
                        retour += ", ";
                    }
                }
                if (Destructeurs.Count != 0)
                    retour += "    Destructeurs :" + "\n";
                for(int i=0;i<Destructeurs.Count;i++)
                {
                    retour += "        -"+Destructeurs[i] + "\n";
                }
                if (Precipiteurs.Count != 0)
                    retour += "    Precipiteurs :" + "\n";
                for (int i = 0; i < Precipiteurs.Count; i++)
                {
                    retour += "        -" + Precipiteurs[i] + "\n";
                }
                if (Catalyseurs.Count != 0)
                    retour += "    Catalyseurs :" + "\n";
                for (int i = 0; i < Catalyseurs.Count; i++)
                {
                    retour += "        -" + Catalyseurs[i] + "\n";
                }
                retour += "    Transformation : Température " + trTempMin + " - " + trTempMax+" Degres, Acidité " + trPhMin + " - " + trPhMax + '\n';
                retour += "    -> " + Resultat+'\n';
                return retour;
            }
        }
        public class Solution
        {
            public List<Compose> Composition;
            public bool Precipite;
            public int Pollution = 0;

            public static Solution SolutionVide()
            {
                Solution retour = new Solution();
                retour.Precipite = false;
                retour.Pollution = 0;
                List<Compose> A = new List<Compose>();
                retour.Composition = A;
                return retour;
            }
            public Solution Copier()
            {
                Solution retour = new Solution();
                retour.Precipite = Precipite;
                retour.Pollution = Pollution;
                List<Compose> A = new List<Compose>();
                foreach(Compose comp in Composition)
                {
                    A.Add(comp);
                }
                retour.Composition = A;
                return retour;
            }

            private bool PeutAgir(Solution S, List<String> Agisseurs)
            {
                foreach(string precip in Agisseurs)
                {
                    if(S.Composition.Contains(Compose.FromString(precip)))
                    {
                        return true;
                    }
                }
                return false;
            }
            private bool PeutCatalyser(Solution S, List<string> Catalyseurs)
            {
                foreach (string precip in Catalyseurs)
                {
                    if ( ! S.Composition.Contains(Compose.FromString(precip)))
                    {
                        return false;
                    }
                }
                return true ;
            }

            public Tuple<Solution,Solution> Distiller(int temp)
            {
                //A reste en solution
                List<Compose> A = new List<Compose>();
                //B est extrait
                List<Compose> B = new List<Compose>();
                foreach (Compose comp in Composition)
                {
                    if(comp.TempMax>=temp)
                    {
                        if(comp.Tevap>=temp)
                        {
                            A.Add(comp);
                        }
                        else
                        {
                            B.Add(comp);
                        }
                    }
                }
                Solution S1 = new Solution();
                S1.Precipite = false;
                S1.Composition = A;
                S1.Pollution = Pollution;
                Solution S2 = new Solution();
                S2.Precipite = false;
                S2.Composition = B;
                S2.Pollution = 0;
                return new Tuple<Solution, Solution>(S1,S2);
            }
            public Tuple<Solution,Solution> Precipiter()
            {
                List<Compose> A = new List<Compose>();
                //B est extrait
                List<Compose> B = new List<Compose>();
                foreach (Compose comp in Composition)
                {
                    if(PeutAgir(this,comp.Precipiteurs))
                    {
                        B.Add(comp);
                    }
                    else
                    {
                        A.Add(comp);
                    }
                }
                int poll = Pollution;
                Solution S1 = new Solution();
                S1.Precipite = false;
                S1.Composition = A;
                S1.Pollution = poll;
                Solution S2 = new Solution();
                S2.Precipite = true;
                S2.Composition = B;
                S2.Pollution = poll;
                return new Tuple<Solution, Solution>(S1, S2);
            }
            public Solution Detruire(int Temp)
            {
                int PH = CalculerPH();
                List<Compose> A = new List<Compose>();
                int nbDestruction = 0;
                foreach(Compose comp in Composition)
                {
                    bool StabiliteChimique = ((!PeutAgir(this, comp.Destructeurs)) && (PH <= comp.PhMax && PH >= comp.PhMin))||Precipite;
                    bool StabiliteThermique = (Temp <= comp.TempMax);
                    if ( StabiliteThermique && StabiliteChimique)
                    {
                        A.Add(comp);
                    }
                    else
                    {
                        nbDestruction++;
                    }
                }
                Solution S1 = new Solution();
                S1.Precipite = Precipite;
                S1.Composition = A;
                S1.Pollution = Pollution + nbDestruction;
                return S1;
            }
            public Solution Transformer(int Temp, ref List<string> ComposesConnus, SerializableDictionary<string, Compose> composes)
            {
                int PH = CalculerPH();
                List<Compose> A = new List<Compose>();
                foreach (Compose comp in Composition)
                {
                    if (PeutCatalyser(this, comp.Catalyseurs) && (Temp < comp.trTempMax && Temp>comp.trTempMin) && (PH < comp.trPhMax && PH > comp.trPhMin))
                    {
                        A.Add(composes[comp.Resultat]);
                        string newNom = comp.Resultat;
                        if(!ComposesConnus.Contains(newNom))
                        {
                            ComposesConnus.Add(newNom);
                        }
                    }
                    else
                    {
                        A.Add(comp);
                    }
                }
                Solution S1 = new Solution();
                S1.Precipite = Precipite;
                S1.Composition = A;
                S1.Pollution = Pollution;
                return S1;
            }
            public Solution Ajouter(Solution sol)
            {
                
                List<Compose> A = new List<Compose>();
                foreach (Compose comp in Composition)
                {
                    A.Add(comp);
                }
                foreach (Compose comp in sol.Composition)
                {
                    if (!A.Contains(comp))
                    {
                        A.Add(comp);
                    }
                }
                Solution S = new Solution();
                S.Precipite = Precipite;
                S.Composition = A;
                S.Pollution = Pollution + sol.Pollution;
                return S;
            }
            public Solution Ajouter(Ingredient ingredient)
            {
                
                List<Compose> A = new List<Compose>();
                foreach (Compose comp in Composition)
                {
                    A.Add(comp);
                }
                foreach (Compose comp in ingredient.PrincipesActifs)
                {
                    if (!A.Contains(comp))
                    {
                        A.Add(comp);
                    }
                }
                Solution S = new Solution();
                S.Precipite = Precipite;
                S.Composition = A;
                S.Pollution = Pollution + ingredient.Pollution;
                return S;
            }
            public int CalculerPH()
            {
                int ph = 20;
                foreach(Compose comp in Composition)
                {
                    ph += comp.deltaPH;
                }
                return ph;
            }
            public List<Tuple<Effets,int>> CalculerEffets()
            {
                Dictionary<Effets, int> effets = new Dictionary<Effets, int>();
                foreach(Effets eff in Enum.GetValues(typeof(Effets)))
                {
                    effets[eff] = 0;
                }
                foreach(Compose comp in Composition)
                {
                    foreach(Effets eff in comp.Effet)
                    {
                        effets[eff] += comp.Complexite;
                    }
                }

                List<Tuple<Effets, int>> result = new List<Tuple<Effets, int>>(); ;

                int stabilite = effets[Effets.Stabilisateur] - effets[Effets.Explosif];
                if(stabilite>0)
                {
                    result.Add(new Tuple<Effets, int>(Effets.Stabilisateur, stabilite));
                }
                if(stabilite<0)
                {
                    result.Add(new Tuple<Effets, int>(Effets.Explosif, -stabilite));
                }

                int sante = effets[Effets.Soin] - effets[Effets.Poison];
                if (sante > 0)
                {
                    result.Add(new Tuple<Effets, int>(Effets.Soin, sante));
                }
                if (sante < 0)
                {
                    result.Add(new Tuple<Effets, int>(Effets.Poison, -sante));
                }

                int odeur = effets[Effets.Parfum] - effets[Effets.Pestilence];
                if (odeur > 0)
                {
                    result.Add(new Tuple<Effets, int>(Effets.Parfum, odeur));
                }
                if (odeur < 0)
                {
                    result.Add(new Tuple<Effets, int>(Effets.Pestilence, -odeur));
                }

                int gout = effets[Effets.Savoureux] - effets[Effets.Immonde];
                if (gout > 0)
                {
                    result.Add(new Tuple<Effets, int>(Effets.Savoureux, gout));
                }
                if (gout < 0)
                {
                    result.Add(new Tuple<Effets, int>(Effets.Immonde, -gout));
                }
                return result;
            }

            public string AfficherEffets()
            {
                List<Tuple<Effets, int>> effets = CalculerEffets();
                string retour = "Effets :" + "\n";
                for (int i = 0; i < effets.Count; i++)
                {
                    retour += "    -" + effets[i].Item1 + " " + effets[i].Item2 + "\n";
                }
                return retour;
            }

            public string Afficher()
            {
                List<Tuple<Effets, int>> effets = CalculerEffets();

                string retour = "    Acidité : " + CalculerPH() + '\n'; ;
                if (Precipite)
                {
                    retour += "    Poudre"+'\n';
                }
                else
                {
                    retour += "    Liquide"+"\n"; 
                }
                retour += "    Composition :" + "\n";
                for (int i = 0; i < Composition.Count; i++)
                {
                    retour += "        -" + Composition[i].Nom + "\n";
                }
                retour += "    Effets :" + "\n";
                for (int i = 0; i < effets.Count; i++)
                {
                    retour += "        -" + effets[i].Item1 +" "+effets[i].Item2+ "\n";
                }
                return retour;
            }
        }
        public class Recette
        {
            public string Nom;
            public int NbIngredients;
            public Solution ProduitFini;
            public int CoutFabrication;
            public int Temperature;
            public void Ajouter(Recette recette)
            {
                if ((recette.ProduitFini.Precipite && ProduitFini.Precipite) || (!(recette.ProduitFini.Precipite) && !(ProduitFini.Precipite)))
                {
                    CoutFabrication += recette.CoutFabrication;
                    ProduitFini = ProduitFini.Ajouter(recette.ProduitFini);
                    NbIngredients += recette.NbIngredients;
                }
            }
            public void Ajouter(Ingredient ingredient)
            {
                if (!ProduitFini.Precipite)
                {
                    CoutFabrication += ingredient.Cout;
                    ProduitFini = ProduitFini.Ajouter(ingredient);
                    NbIngredients += 1;
                }
            }
            public void ChangerTemperature(int nextTemp)
            {
                Temperature = nextTemp;
                CoutFabrication++;
            }
            public void Distiller(int Temp)
            {
                ProduitFini = ProduitFini.Distiller(Temp).Item2;
                CoutFabrication++;
            }
            public void Evaporer(int Temp)
            {
                ProduitFini = ProduitFini.Distiller(Temp).Item1;
                CoutFabrication++;
            }
            //1
            public void Detruire()
            {
                ProduitFini = ProduitFini.Detruire(Temperature);
            }
            //2
            public void Transformer(ref List<string> ComposesConnus, SerializableDictionary<string, Compose> composes)
            {
                ProduitFini = ProduitFini.Transformer(Temperature, ref ComposesConnus,composes);
            }
            //3
            public void Precipiter(bool garderPrecipite)
            {
                if(garderPrecipite)
                {
                    ProduitFini = ProduitFini.Precipiter().Item2;
                }
                else
                {
                    ProduitFini = ProduitFini.Precipiter().Item1;
                }

            }

            public static Recette RecetteVide()
            {
                Recette retour = new Recette();
                retour.Nom = "Eau";
                retour.NbIngredients = 0;
                retour.CoutFabrication = 0;
                retour.Temperature = 20;
                retour.ProduitFini = Solution.SolutionVide();
                return retour;

            }
            public Recette Copier()
            {
                Recette retour = new Recette();
                retour.Nom = Nom;
                retour.NbIngredients = NbIngredients;
                retour.CoutFabrication = CoutFabrication;
                retour.Temperature = Temperature;
                retour.ProduitFini = ProduitFini.Copier();
                return retour;
            }

            public int PrixDeVente()
            {
                int result = 0;
                List<Tuple<Effets, int>> eff = ProduitFini.CalculerEffets();
                foreach (Tuple<Effets, int> e in eff)
                {
                    result += e.Item2;
                }
                if(ProduitFini.Precipite)
                {
                    result *= 2;
                }
                return result;
            }

            public string Afficher()
            {
                string retour = Nom + " ( " + NbIngredients + " Ingredients)" + " :" + '\n';
                retour += "    Prix de Fabrication/Vente : " + CoutFabrication +"/"+PrixDeVente()+  '\n';
                retour += "    Pollution : " + ProduitFini.Pollution + '\n';
                return retour;
            }
            public string Afficher_Long()
            {
                string retour = Afficher() + '\n';
                retour += "    Temperature de fabrication : "+Temperature+" Degres" + '\n';
                retour += ProduitFini.Afficher();
                return retour;
            }
        }
        public class Ingredient
        {
            public string Nom;
            public int Pollution;
            public List<Compose> PrincipesActifs;
            public int Cout;
            public int Prestige;


            public string Afficher()
            {
                string retour = Nom + " < Prestige " + Prestige + " >" + " :" + '\n';
                retour += "    Cout : " + Cout + '\n';
                retour += "    Pollution : " + Pollution + '\n';
                retour += "    Principes Actifs :" + "\n";
                for (int i = 0; i < PrincipesActifs.Count; i++)
                {
                    retour += "        -" + PrincipesActifs[i].Nom + "\n";
                }
                return retour;
            }
        }
        public class Laboratoire
        {

            public void Setup(int seed)
            {
                r = new Random(seed);
                Xp = 0;
                etape = 0;
                recettes = new SerializableDictionary<string, Recette>();
                ingredients = new SerializableDictionary<string, Ingredient>();
                Prestige = 1;
                Fonds = 50;
                composes = new SerializableDictionary<string, Compose>();
                recetteCourante = Recette.RecetteVide();
                missionActuelle = Mission.Generer(1, ref r);
                composesConnus = new List<string>();
                NbMissionsSkip = 0;
                GenererChimie(30, 25, 30);
            }
            public Laboratoire(int seed)
            {
                Setup(seed);
            }
            public Laboratoire()
            {
                Setup(0);
            }
            public int etape;
            Random r;
            public int Xp;
            public int Prestige;
            public int Fonds;
            public SerializableDictionary<string,Recette> recettes ;
            public SerializableDictionary<string, Ingredient> ingredients;
            public SerializableDictionary<string, Compose> composes ;
            public Recette recetteCourante ;
            public Mission missionActuelle;
            public List<string> composesConnus ;


            private int NbMissionsSkip;

            public string AfficherIngredients()
            {
                string retour = "";
                foreach(KeyValuePair<string,Ingredient> ing in ingredients)
                {
                    if(ing.Value.Prestige<= Prestige)
                    {
                        retour += ing.Value.Afficher();
                    }
                }
                return retour;
            }
            public List<string> GetNomsIngredients()
            {
                List<string> retour = new List<string>();
                foreach (KeyValuePair<string, Ingredient> ing in ingredients)
                {
                    if (ing.Value.Prestige <= Prestige)
                    {
                        retour.Add(ing.Value.Nom);
                        List<Compose> PA = ing.Value.PrincipesActifs;
                        foreach(Compose p in PA)
                        {
                            if(!composesConnus.Contains(p.Nom))
                            {
                                composesConnus.Add(p.Nom);
                            }
                        }
                    }
                }
                return retour;
            }
            public string AfficherRecettes()
            {
                string retour = "";
                foreach (KeyValuePair<string, Recette> ing in recettes)
                {
                    retour += ing.Value.Afficher();
                }
                return retour;
            }
            public string AfficherComposes()
            {
                string retour = "";
                foreach (KeyValuePair<string, Compose> ing in composes)
                {
                    if (composesConnus.Contains(ing.Key))
                    {
                        retour += ing.Value.Afficher();
                    }
                }
                return retour;
            }
            public string AfficherMission()
            {
                return missionActuelle.Afficher();
            }

            public void StockerRecette(string Nom)
            {
                recettes[Nom] = recetteCourante.Copier();
            }
            public void OuvrirRecette(string Nom)
            {
                recetteCourante = recettes[Nom].Copier();
            }
            public void SupprimerRecette(string Nom)
            {
                recettes.Remove(Nom);
            }

            public void GagnerXp()
            {
                Xp++;
                while(Xp>Prestige+1)
                {
                    Xp -= (Prestige + 1);
                    Prestige++;
                }
            }
            public void ViderPlanTravail()
            {
                recetteCourante = Recette.RecetteVide();
            }

            public void AjouterIngredient(string Nom)
            {
                if (ingredients[Nom].Prestige <= Prestige)
                {
                    recetteCourante.Ajouter(ingredients[Nom]);
                    Fonds -= ingredients[Nom].Cout; 
                }
            }
            public void AjouterRecette(string Nom)
            {
                recetteCourante.Ajouter(recettes[Nom]);
                Fonds -= recettes[Nom].CoutFabrication;
            }

            public void NouvelleMission()
            {
                NbMissionsSkip++;
                missionActuelle = Mission.Generer(Math.Max(1,Prestige),ref r);
                if(NbMissionsSkip>5)
                {
                    Prestige = Math.Max(0,Prestige-1);
                    NbMissionsSkip = 0;
                }
            }
            public bool ValiderMission(string NomRecette)
            {
                if(recettes.ContainsKey(NomRecette) && missionActuelle.verifierConformite(recettes[NomRecette]))
                {
                    GagnerXp();
                    Fonds += recettes[NomRecette].PrixDeVente() + missionActuelle.Bonus;
                    NbMissionsSkip = 0;
                    return true;
                }
                return false;
            }

            public void Action_Evaporer(int Temperature)
            {
                if (!recetteCourante.ProduitFini.Precipite)
                {
                    recetteCourante.Evaporer(Temperature);
                    Fonds -= 1;
                }
            }
            public void Action_Distiller(int Temperature)
            {
                if (!recetteCourante.ProduitFini.Precipite)
                {
                    recetteCourante.Distiller(Temperature);
                    Fonds -= 1;
                }
            }
            public void Action_ChangerTemp(int Temperature)
            {
                recetteCourante.ChangerTemperature(Temperature);
                Fonds -= 1;
            }

            public void Step_Detruire()
            {
                recetteCourante.Detruire();
            }
            public void Step_Transformer()
            {
                if (!recetteCourante.ProduitFini.Precipite)
                {
                    recetteCourante.Transformer(ref composesConnus,composes);
                }
            }
            public void Step_Precipiter(bool garderPrecipite)
            {
                if (!recetteCourante.ProduitFini.Precipite)
                {
                    recetteCourante.Precipiter(garderPrecipite);
                }
            }


            public void GenererChimie(int ComplexiteMax, int composantsMaxPaxComplexite, int ingredientsMaxParComplexite)
            {
                missionActuelle = Mission.Generer(1, ref r);
                List<string> NomsUtilises = new List<string>();
                //1 generer les composes par complexite decroissante
                List<Compose>[] hierarchie = new List<Compose>[ComplexiteMax+1];
                hierarchie[ComplexiteMax] = new List<Compose>();
                //Descendre et trouver les suites
                for(int i=ComplexiteMax;i>=1;i--)
                {
                    List<Compose> composeActuel = new List<Compose>();
                    int nbComp = r.Next(composantsMaxPaxComplexite / 2, composantsMaxPaxComplexite + 1);
                    for(int j=0;j<nbComp;j++)
                    {
                        Compose cmp = Compose.GenererAleatoire(i, ref r, ref NomsUtilises);
                        if (hierarchie[i].Count > 0)
                        {
                            cmp.Resultat = hierarchie[i].ElementAt(r.Next(0, hierarchie[i].Count)).Nom;
                        }
                        else
                        {
                            cmp.Resultat = cmp.Nom;
                        }
                        composeActuel.Add(cmp);
                    }
                    hierarchie[i-1] = composeActuel;
                }
                //Ajouter les prerequis
                for (int i = ComplexiteMax; i >= 1; i--)
                {
                    List<Compose> cmp = hierarchie[i - 1];
                    for(int j=0;j<cmp.Count;j++)
                    {
                        Compose c = cmp.ElementAt(j);
                        int nbDestructuers = r.Next(0, i + 1);
                        int nbCatalyseurs = r.Next(0, i + 1);
                        int nbPrecipiteurs = r.Next(1, i + 1);
                        for(int k=0;k<nbDestructuers;k++)
                        {
                            int h = r.Next(0, i);
                            int l = r.Next(0, hierarchie[h].Count);
                            Compose aAj = hierarchie[h].ElementAt(l);
                            if (!(c.Destructeurs.Contains(aAj.Nom) || c.Equals(aAj)))
                            {
                                c.Destructeurs.Add(aAj.Nom);
                            }
                        }
                        for (int k = 0; k < nbPrecipiteurs; k++)
                        {
                            int h = r.Next(0, i);
                            int l = r.Next(0, hierarchie[h].Count);
                            Compose aAj = hierarchie[h].ElementAt(l);
                            if (!(c.Precipiteurs.Contains(aAj.Nom)|| c.Destructeurs.Contains(aAj.Nom) || c.Equals(aAj)))
                            {
                                c.Precipiteurs.Add(aAj.Nom);
                            }
                        }
                        for (int k = 0; k < nbCatalyseurs; k++)
                        {
                            int h = r.Next(0, i);
                            int l = r.Next(0, hierarchie[h].Count);
                            Compose aAj = hierarchie[h].ElementAt(l);
                            if (!(c.Catalyseurs.Contains(aAj.Nom)|| c.Precipiteurs.Contains(aAj.Nom) || c.Destructeurs.Contains(aAj.Nom)|| c.Equals(aAj)))
                            {
                                c.Catalyseurs.Add(aAj.Nom);
                            }
                        }
                        composes[c.Nom] = c;
                        if(i==1)
                        {
                            composesConnus.Add(c.Nom);
                        }
                    }
                }
                //2 generer les ingredients
                for (int i = 1; i < ComplexiteMax; i++)
                {
                    int nbIng = r.Next(ingredientsMaxParComplexite / 2, ingredientsMaxParComplexite + 1);
                    for (int j = 0; j < nbIng; j++)
                    {
                        Ingredient ing = new Ingredient();
                        string nom = GenerateurDeNom.genererNomPropre(ref r);
                        while (NomsUtilises.Contains(nom))
                        {
                            nom = GenerateurDeNom.genererNomPropre(ref r);
                        }
                        ing.Nom = nom;
                        ing.Pollution = r.Next(0, 2 * i);
                        ing.Prestige = 2 * (i-1);
                        ing.Cout = r.Next(1, 3 * i);
                        ing.PrincipesActifs = new List<Compose>();
                        int nbPrincipesActifs = r.Next(1, i+1);
                        for(int k=0;k<nbPrincipesActifs;k++)
                        {
                            int l = r.Next(0, i);
                            if (hierarchie[l].Count > 0)
                            {
                                Compose com = hierarchie[l].ElementAt(r.Next(0, hierarchie[l].Count));
                                ing.PrincipesActifs.Add(com);
                            }
                        }
                        ingredients[ing.Nom] = ing;
                    }
                }

            }
        }
        public class Mission
        {
            public int PrestigeMini;
            public List<EffTuple> EffetsRequis;
            public List<EffTuple> EffetsInterdits;
            public int PollutionMax;
            public int CoutMax;
            public int Bonus;
            public bool EnPoudre;
            
            public bool verifierConformite(Recette recette)
            {
                bool precip = recette.ProduitFini.Precipite == EnPoudre;
                bool pollution = recette.ProduitFini.Pollution <= PollutionMax;
                bool prix = recette.CoutFabrication <= CoutMax;
                bool effets = true;
                bool effetsInterdit = true;
                List<Tuple<Effets, int>> effetsProduit = recette.ProduitFini.CalculerEffets();
                foreach (EffTuple eff in EffetsRequis)
                {
                    bool ok = false;
                    foreach (Tuple<Effets, int> eff_ in effetsProduit)
                    {
                        if(eff_.Item1 == eff.Item1 && eff_.Item2>= eff.Item2)
                        {
                            ok = true;
                            break;
                        }
                    }
                    if(!ok)
                    {
                        effets = false;
                        break;
                    }
                }
                foreach (EffTuple eff in EffetsInterdits)
                {
                    bool ok = false;
                    foreach (Tuple<Effets, int> eff_ in effetsProduit)
                    {
                        if (eff_.Item1 == eff.Item1 && eff_.Item2 <= eff.Item2)
                        {
                            ok = true;
                            break;
                        }
                    }
                    if (!ok)
                    {
                        effetsInterdit = false;
                        break;
                    }
                }
                return precip && pollution && prix && effets && effetsInterdit;
            }
            public string Afficher()
            {
                string retour = "Mission de Prestige " + PrestigeMini + '\n';
                retour += "    BONUS : " + Bonus +" Credits"+ '\n';
                retour += "    Pollution <= " + PollutionMax + '\n';
                retour += "    Cout <= " + CoutMax + '\n';
                if(EnPoudre)
                {
                    retour += "    Etat : Poudre" + '\n';
                }
                else
                {
                    retour += "    Etat : Liquide" + '\n';
                }
                retour += "    Effets :" + "\n";
                for (int i = 0; i < EffetsRequis.Count; i++)
                {
                    retour += "        -" + EffetsRequis[i].Item1 + " >= " + EffetsRequis[i].Item2 + "\n";
                }
                for (int i = 0; i < EffetsInterdits.Count; i++)
                {
                    retour += "        -" + EffetsInterdits[i].Item1 + " <= " + EffetsInterdits[i].Item2 + "\n";
                }
                return retour;
            }
            public static Mission Generer(int PrestigeMax, ref Random r)
            {
                Mission retour = new Mission();
                retour.EnPoudre = r.Next(0, 2) == 0;
                retour.PrestigeMini = r.Next(1, PrestigeMax + 1);
                retour.PollutionMax = r.Next(0, retour.PrestigeMini * 2+4);
                retour.CoutMax = r.Next(1+retour.PrestigeMini, 5+retour.PrestigeMini * 2);
                retour.Bonus = r.Next(2 * retour.PrestigeMini, 6* retour.PrestigeMini);
                retour.EffetsRequis = new List<EffTuple>();
                retour.EffetsInterdits = new List<EffTuple>();
                List<Effets> EffetsDispos = ((Effets[])Enum.GetValues(typeof(Effets))).ToList<Effets>();
                int NbPointsRestants = retour.PrestigeMini;
                while(NbPointsRestants>0 && EffetsDispos.Count>0)
                {
                    bool bornesup = false;
                    Effets choisi = EffetsDispos[r.Next(0, EffetsDispos.Count)];
                    if (r.Next(0, 3) == 0)
                    {
                        bornesup = true;
                    }
                    if (choisi ==Effets.Explosif || choisi == Effets.Stabilisateur)
                    {
                        EffetsDispos.Remove(Effets.Explosif);
                        EffetsDispos.Remove(Effets.Stabilisateur);
                    }
                    if (choisi == Effets.Soin || choisi == Effets.Poison)
                    {
                        EffetsDispos.Remove(Effets.Soin);
                        EffetsDispos.Remove(Effets.Poison);
                    }
                    if (choisi == Effets.Parfum || choisi == Effets.Pestilence)
                    {
                        EffetsDispos.Remove(Effets.Parfum);
                        EffetsDispos.Remove(Effets.Pestilence);
                    }
                    if (choisi == Effets.Savoureux || choisi == Effets.Immonde)
                    {
                        EffetsDispos.Remove(Effets.Savoureux);
                        EffetsDispos.Remove(Effets.Immonde);
                    }
                    int Intensite = r.Next(1, NbPointsRestants);
                    NbPointsRestants -= Intensite;
                    if(bornesup)
                    {
                        retour.EffetsInterdits.Add(new EffTuple(choisi, r.Next(Intensite, Math.Max(Intensite,retour.PrestigeMini*2 + 3))));
                    }
                    retour.EffetsRequis.Add(new EffTuple(choisi, Intensite));
                }
                if (r.Next(0, 2) == 0)
                {
                    NbPointsRestants = r.Next(0, retour.PrestigeMini / 2 + 1);
                    retour.Bonus += 2 * NbPointsRestants;
                    while (NbPointsRestants > 0 && EffetsDispos.Count > 0)
                    {
                        Effets choisi = EffetsDispos[r.Next(0, EffetsDispos.Count)];
                        if (choisi == Effets.Explosif || choisi == Effets.Stabilisateur)
                        {
                            EffetsDispos.Remove(Effets.Explosif);
                            EffetsDispos.Remove(Effets.Stabilisateur);
                        }
                        if (choisi == Effets.Soin || choisi == Effets.Poison)
                        {
                            EffetsDispos.Remove(Effets.Soin);
                            EffetsDispos.Remove(Effets.Poison);
                        }
                        if (choisi == Effets.Parfum || choisi == Effets.Pestilence)
                        {
                            EffetsDispos.Remove(Effets.Parfum);
                            EffetsDispos.Remove(Effets.Pestilence);
                        }
                        if (choisi == Effets.Savoureux || choisi == Effets.Immonde)
                        {
                            EffetsDispos.Remove(Effets.Savoureux);
                            EffetsDispos.Remove(Effets.Immonde);
                        }
                        int Intensite = r.Next(0, NbPointsRestants);
                        NbPointsRestants -= Intensite;
                        retour.EffetsInterdits.Add(new EffTuple(choisi, retour.PrestigeMini - Intensite));
                    }
                }
                return retour;
            }
        }

        private void IngredientsLB_SelectedIndexChanged(object sender, EventArgs e)
        {
            MajIngredients();
            MajInfos();
        }

        private void PrincipesLB_SelectedIndexChanged(object sender, EventArgs e)
        {
            MajPrincipes();
        }

        private void RecettesLB_SelectedIndexChanged(object sender, EventArgs e)
        {
            MajRecettes();
            MajInfos();
        }

        private void bChangerMission_Click(object sender, EventArgs e)
        {
            lab.NouvelleMission();
            tBRAV.Text = "";
            MajMission();
            MajInfos();
        }

        private void bValiderMission_Click(object sender, EventArgs e)
        {
            if(lab.ValiderMission(tBRAV.Text))
            {
                tBRAV.Text = "";
                lab.NouvelleMission();
                
            }
            else
            {
                tBRAV.Text = "Nom de recette valide...";
            }
            MajMission();
        }

        private void bVider_Click(object sender, EventArgs e)
        {
            lab.ViderPlanTravail();
            MajEprouvette();
            MajInfos();
        }

        private void bAjIng_Click(object sender, EventArgs e)
        {
            string nom = "";
            if (IngredientsLB.SelectedIndex > -1)
            {
                nom = IngredientsLB.SelectedItem.ToString();
                lab.AjouterIngredient(nom);
            }
            MajPrestigeEtCredits();
            MajEprouvette();
        }

        private void bAjRec_Click(object sender, EventArgs e)
        {
            string nom = "";
            if (RecettesLB.SelectedIndex > -1)
            {
                nom = RecettesLB.SelectedItem.ToString();
                lab.AjouterRecette(nom);
            }
            MajPrestigeEtCredits();
            MajEprouvette();
        }

        private void bGarder_Click(object sender, EventArgs e)
        {
            string nom = tBnomRecette.Text;
            if (nom != "")
            {
                lab.StockerRecette(nom);
                tBnomRecette.Text = "Recette Experimentale";
                bOublier.Enabled = true;
            }
            else
            {
                tBnomRecette.Text = "Nom non vide";
            }
            MajRecettes();
        }

        private void bOublier_Click(object sender, EventArgs e)
        {
            string nom = "";
            if (RecettesLB.SelectedIndex > -1)
            {
                nom = RecettesLB.SelectedItem.ToString();
                if ( (MessageBox.Show("Voulez vous oublier la recette "+nom+" ?", "Vers l'Oubli...",
    MessageBoxButtons.YesNo, MessageBoxIcon.Question,
    MessageBoxDefaultButton.Button1) == System.Windows.Forms.DialogResult.Yes))
                {
                    lab.SupprimerRecette(nom);
                    RecettesLB.Items.Remove(nom);
                    if (RecettesLB.Items.Count == 0)
                    {
                        bOublier.Enabled = false;
                    }
                }
            }
            MajRecettes();
        }

        private void tBnomRecette_TextChanged(object sender, EventArgs e)
        {
            lab.recetteCourante.Nom = tBnomRecette.Text;
            MajEprouvette();
        }

        private void bEtapeSuivante_Click(object sender, EventArgs e)
        {
            if (lab.etape == 0)
            {
                lab.etape++;
            }
            else if (lab.etape == 1)
            {
                lab.Step_Detruire();
                lab.etape++;
            }
            else if (lab.etape == 2)
            {
                lab.Step_Transformer();
                lab.etape++;
            }
            else if (lab.etape == 3)
            {
                lab.Step_Precipiter(chBGardPrecip.Checked);
                lab.etape=0;
            }
            MajEprouvette();
            MajEtape();
        }

        private void trackBarTemp_Scroll(object sender, EventArgs e)
        {
            MajTemperatures();
        }

        private void trackBarEvap_Scroll(object sender, EventArgs e)
        {
            MajTemperatures();
        }

        private void trackBarDist_Scroll(object sender, EventArgs e)
        {
            MajTemperatures();
        }

        private void breg_Click(object sender, EventArgs e)
        {
            lab.Action_ChangerTemp(trackBarTemp.Value);
            MajEprouvette();
            MajPrestigeEtCredits();
        }

        private void bev_Click(object sender, EventArgs e)
        {
            lab.Action_Evaporer(trackBarEvap.Value);
            MajEprouvette();
            MajPrestigeEtCredits();
        }

        private void bdist_Click(object sender, EventArgs e)
        {
            lab.Action_Distiller(trackBarDist.Value);
            MajEprouvette();
            MajPrestigeEtCredits();
        }

        private void Enregistrer(string Path)
        {
            XmlSerializer xs = new XmlSerializer(typeof(Laboratoire));
            using (System.IO.StreamWriter wr = new System.IO.StreamWriter(Path))
            {
                xs.Serialize(wr, lab);
            }
        }
        private void Charger(string Path)
        {
            XmlSerializer xs = new XmlSerializer(typeof(Laboratoire));
            using (System.IO.StreamReader rd = new System.IO.StreamReader(Path))
            {
                lab = xs.Deserialize(rd) as Laboratoire;
            }
            Maj();
        }

        private void benregistrerSous_Click(object sender, EventArgs e)
        {
            SaveFileDialog saveFileDialog1 = new SaveFileDialog();
            saveFileDialog1.Filter = "Fichier Chaudron|*.chdr";
            saveFileDialog1.Title = "Sauver la partie";
            saveFileDialog1.ShowDialog();
            if(saveFileDialog1.FileName != "")
            {
                Enregistrer(saveFileDialog1.FileName);
            }
        }

        private void bchargerSauv_Click(object sender, EventArgs e)
        {
            OpenFileDialog openFileDialog1 = new OpenFileDialog()
            {
                FileName = "Sauvegarde ",
                Filter = "Fichiers Chaudron  (*.chdr)|*.chdr",
                Title = "Ouvrir une sauvegarde"
            };
            if(openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                Charger(openFileDialog1.FileName);
            }
        }

        private void bnouvSeed_Click(object sender, EventArgs e)
        {
            int seed = textBoxnouvSeed.Text.GetHashCode();
            if ((MessageBox.Show("Voulez vous remplacer la partie actuelle par la partie de Seed \"" + textBoxnouvSeed.Text + "\" ?", "Nouveau Départ !",
    MessageBoxButtons.YesNo, MessageBoxIcon.Question,
    MessageBoxDefaultButton.Button1) == System.Windows.Forms.DialogResult.Yes))
            {
                lab = new Laboratoire(seed);
                Maj();
            }
        }






        public class EffTuple
        {
            public Effets Item1;
            public int Item2;

            public EffTuple(Effets item1, int item2)
            {
                Item1 = item1;
                Item2 = item2;
            }
            public EffTuple()
            {
                Item1 = Effets.Explosif;
                Item2 = 0;
            }
        }

        [XmlRoot("dictionary")]
        public class SerializableDictionary<TKey, TValue>
    : Dictionary<TKey, TValue>, IXmlSerializable
        {
            public SerializableDictionary() { }
            public SerializableDictionary(IDictionary<TKey, TValue> dictionary) : base(dictionary) { }
            public SerializableDictionary(IDictionary<TKey, TValue> dictionary, IEqualityComparer<TKey> comparer) : base(dictionary, comparer) { }
            public SerializableDictionary(IEqualityComparer<TKey> comparer) : base(comparer) { }
            public SerializableDictionary(int capacity) : base(capacity) { }
            public SerializableDictionary(int capacity, IEqualityComparer<TKey> comparer) : base(capacity, comparer) { }

            #region IXmlSerializable Members
            public System.Xml.Schema.XmlSchema GetSchema()
            {
                return null;
            }

            public void ReadXml(System.Xml.XmlReader reader)
            {
                XmlSerializer keySerializer = new XmlSerializer(typeof(TKey));
                XmlSerializer valueSerializer = new XmlSerializer(typeof(TValue));

                bool wasEmpty = reader.IsEmptyElement;
                reader.Read();

                if (wasEmpty)
                    return;

                while (reader.NodeType != System.Xml.XmlNodeType.EndElement)
                {
                    reader.ReadStartElement("item");

                    reader.ReadStartElement("key");
                    TKey key = (TKey)keySerializer.Deserialize(reader);
                    reader.ReadEndElement();

                    reader.ReadStartElement("value");
                    TValue value = (TValue)valueSerializer.Deserialize(reader);
                    reader.ReadEndElement();

                    this.Add(key, value);

                    reader.ReadEndElement();
                    reader.MoveToContent();
                }
                reader.ReadEndElement();
            }

            public void WriteXml(System.Xml.XmlWriter writer)
            {
                XmlSerializer keySerializer = new XmlSerializer(typeof(TKey));
                XmlSerializer valueSerializer = new XmlSerializer(typeof(TValue));

                foreach (TKey key in this.Keys)
                {
                    writer.WriteStartElement("item");

                    writer.WriteStartElement("key");
                    keySerializer.Serialize(writer, key);
                    writer.WriteEndElement();

                    writer.WriteStartElement("value");
                    TValue value = this[key];
                    valueSerializer.Serialize(writer, value);
                    writer.WriteEndElement();

                    writer.WriteEndElement();
                }
            }
            #endregion
        }


        string paSrch = "";
        int paIndSrch = 0;
        List<string> FindIng = new List<string>();
        private void bSrchPPA_Click(object sender, EventArgs e)
        {
            if (PrincipesLB.SelectedIndex > -1)
            {
                string cmp = PrincipesLB.SelectedItem.ToString();
                if (cmp != paSrch)
                {
                    paSrch = cmp;
                    FindIng = new List<string>();
                    paIndSrch = 0;
                    foreach (string nom in IngredientsLB.Items)
                    {
                        Ingredient ing = lab.ingredients[nom];
                        if(ing.PrincipesActifs.Contains(lab.composes[cmp]))
                        {
                            FindIng.Add(nom);
                        }
                    }
                }
                else
                {
                    if (FindIng.Count() != 0)
                    {
                        paIndSrch = (paIndSrch + 1) % FindIng.Count();
                    }
                }
                if (FindIng.Count() != 0)
                {
                    IngredientsLB.SelectedItem = FindIng.ElementAt(paIndSrch);
                }
            }
            
        }

        string ingSrch = "";
        int ingIndSrch = 0;
        List<string> FindPa = new List<string>();
        private void bsrchPI_Click(object sender, EventArgs e)
        {
            if (IngredientsLB.SelectedIndex > -1)
            {
                string ing = IngredientsLB.SelectedItem.ToString();
                if (ing != ingSrch)
                {
                    ingSrch = ing;
                    FindPa = new List<string>();
                    ingIndSrch = 0;
                    List<Compose> comp = lab.ingredients[ing].PrincipesActifs;
                    foreach(Compose c in comp)
                    {
                        FindPa.Add(c.Nom);
                    }
                }
                else
                {
                    if (FindPa.Count() != 0)
                    {
                        ingIndSrch = (ingIndSrch + 1) % FindPa.Count();
                    }
                }
                if (FindPa.Count() != 0)
                {
                    PrincipesLB.SelectedItem = FindPa.ElementAt(ingIndSrch);
                }
            }
        }

        private void chBGardPrecip_CheckedChanged(object sender, EventArgs e)
        {
            MajInfos();
        }

        private void tbSearch_TextChanged(object sender, EventArgs e)
        {
            string txt = tbSearch.Text;
            foreach(string atester in IngredientsLB.Items)
            {
                if(atester.StartsWith(txt, StringComparison.CurrentCultureIgnoreCase))
                {
                    IngredientsLB.SelectedItem = atester;
                    break;
                }
            }
            foreach (string atester in RecettesLB.Items)
            {
                if (atester.StartsWith(txt, StringComparison.CurrentCultureIgnoreCase))
                {
                    RecettesLB.SelectedItem = atester;
                    break;
                }
            }
            foreach (string atester in PrincipesLB.Items)
            {
                if (atester.StartsWith(txt, StringComparison.CurrentCultureIgnoreCase))
                {
                    PrincipesLB.SelectedItem = atester;
                    break;
                }
            }
            foreach (string atester in ContenuLB.Items)
            {
                if (atester.StartsWith(txt, StringComparison.CurrentCultureIgnoreCase))
                {
                    ContenuLB.SelectedItem = atester;
                    break;
                }
            }
        }

        private void tbSearch_Click(object sender, EventArgs e)
        {
            tbSearch.Text = "";
        }

        private void ContenuLB_SelectedIndexChanged(object sender, EventArgs e)
        {
            PrincipesLB.SelectedItem = ContenuLB.SelectedItem;
        }

        private void tBRAV_TextChanged(object sender, EventArgs e)
        {
            if(tBRAV.Focused)
            {
                if(lab.recettes.ContainsKey(tBRAV.Text))
                {
                    if(lab.missionActuelle.verifierConformite(lab.recettes[RecettesLB.SelectedItem.ToString()]))
                    {
                        bValiderMission.Enabled = true;
                    }
                    else
                    {
                        bValiderMission.Enabled = false;
                    }
                }
                else
                {
                    bValiderMission.Enabled = false;
                }
            }
        }
        public static void SetDoubleBuffered(System.Windows.Forms.Control c)
        {
            if (System.Windows.Forms.SystemInformation.TerminalServerSession)
                return;
            System.Reflection.PropertyInfo aProp = typeof(System.Windows.Forms.Control).GetProperty("DoubleBuffered",
            System.Reflection.BindingFlags.NonPublic |
            System.Reflection.BindingFlags.Instance);
            aProp.SetValue(c, true, null);
        }
    }
}
