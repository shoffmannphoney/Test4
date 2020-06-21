using System;
using System.Collections.Generic;


namespace advtest
{
    [Serializable]
    public static class Grammar
    {
        private static ItemList Items { get; set; }
        private static PersonList Persons { get; set; }
        private static AdvData A { get; set; }
        private static VTList VT { get; set; }

        public static void Init( AdvData pA, VTList pVT,ItemList pItems, PersonList pPersons )
        {
            Items = pItems;
            Persons = pPersons;
            A = pA;
            VT = pVT;
        }
        public static string GetReflexivePronoun(int PersonID, int Case)
        {
            string s = "";
            if (Case == Co.CASE_AKK)
            {
                if (PersonID == A.P_I)
                    s = "mich";
                else if (PersonID == A.P_You)
                    s = "dich";
                else
                    s = "sich";
            }
            else if (Case == Co.CASE_DAT)
            {
                if (PersonID == A.P_I)
                    s = "mir";
                else if (PersonID == A.P_You)
                    s = "dir";
                else
                    s = "sich";

            }
            return s;
        }
        public static string GetPossesivePronoun(int PersonID, int Case)
        {
            Person person = Persons.Find(PersonID);
            string s = "";
            if (Case == Co.CASE_AKK)
            {

                if (PersonID == A.P_I)
                    s = "mein";
                else if (PersonID == A.P_You)
                    s = "dein";
                else if ((person.Sex == Co.SEX_MALE) || (person.Sex == Co.SEX_NEUTER))
                    s = "sein";
                else
                    s = "ihr";
            }
            if (Case == Co.CASE_NOM)
            {
                if (PersonID == A.P_I)
                    s = "mein";
                else if (PersonID == A.P_You)
                    s = "dein";
                else if ((person.Sex == Co.SEX_MALE) || (person.Sex == Co.SEX_NEUTER))
                    s = "sein";
                else
                    s = "ihr";
            }
            else if (Case == Co.CASE_DAT)
            {
                if (PersonID == A.P_I)
                    s = "meiner";
                else if (PersonID == A.P_You)
                    s = "deiner";
                else if ((person.Sex == Co.SEX_MALE) || (person.Sex == Co.SEX_NEUTER))
                    s = "seiner";
                else
                    s = "ihrer";
            }
            else if (Case == Co.CASE_GEN)
            {
                if (PersonID == A.P_I)
                    s = "meines";
                else if (PersonID == A.P_You)
                    s = "deines";
                else if ((person.Sex == Co.SEX_MALE) || (person.Sex == Co.SEX_NEUTER))
                    s = "seines";
                else
                    s = "ihres";
            }
            return s;
        }

        public static string GetPronoun(int PersonID)
        {
            Person person = Persons.Find(PersonID);

            if (PersonID == A.P_I) return ("ich");
            if (PersonID == A.P_You) return ("du");
            if (person.Sex == Co.SEX_FEMALE) return ("sie");
            if (person.Sex == Co.SEX_MALE) return ("er");
            if (person.Sex == Co.SEX_NEUTER) return ("es");
            if (person.Sex == Co.SEX_FEMALE_PL) return ("sie");
            if (person.Sex == Co.SEX_MALE) return ("sie");
            if (person.Sex == Co.SEX_NEUTER) return ("sie");
            return ("sie");
        }

        public static string GetVerbDeclination(int VerbID, int PersonID, int Tense)
        {
            string s = "";
            if (Persons == null) return s;

            Person person = Persons.Find(PersonID);

            if (Tense == Co.CB.Tense_Present)
            {
                if (PersonID == A.P_I) Tense = Co.CB.Tense_1P_Sin_Present;
                else if (PersonID == A.P_You) Tense = Co.CB.Tense_2P_Sin_Present;
                else Tense = Co.CB.Tense_3P_Sin_Present;
            }
            else if (Tense == Co.CB.Tense_Past)
            {
                if (PersonID == A.P_I) Tense = Co.CB.Tense_1P_Sin_Past;
                else if (PersonID == A.P_You) Tense = Co.CB.Tense_2P_Sin_Past;
                else Tense = Co.CB.Tense_3P_Sin_Past;
            }

            for (int i = 0; i < VT.List.Count; i++)
            {
                if (VT.List[i].ID == VerbID)
                {
                    s = VT.List[i].VerbNameTense[Tense];
                }
            }
            return s;  
        }
        public static string GetVerbDeclination(int VerbID, AbstractAdvObject AO, int Tense)
        {
            string s = "";

            if (AO == null)
                return s;


            if (Tense == Co.CB.Tense_Present)
            {
                if (AO.ID == A.P_I) Tense = Co.CB.Tense_1P_Sin_Present;
                else if (AO.ID == A.P_You) Tense = Co.CB.Tense_2P_Sin_Present;
                else Tense = Co.CB.Tense_3P_Sin_Present;
            }
            else if (Tense == Co.CB.Tense_Past)
            {
                if (AO.ID == A.P_I) Tense = Co.CB.Tense_1P_Sin_Past;
                else if (AO.ID == A.P_You) Tense = Co.CB.Tense_2P_Sin_Past;
                else Tense = Co.CB.Tense_3P_Sin_Past;
            }

            for (int i = 0; i < VT.List.Count; i++)
            {
                if (VT.List[i].ID == VerbID)
                {
                    s = VT.List[i].VerbNameTense[Tense];
                }
            }
            return s;
        }
        public static void SetPersons(PersonList pPersons)
        {
            Persons = pPersons;
        }
    }
}