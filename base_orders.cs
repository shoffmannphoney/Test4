using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;

namespace advtest
{
    [Serializable]
    public abstract class AbstractOrder
    {
        protected AdvData A { get; set; }
        [NonSerialized] protected Adv AdvGame;
        protected VerbList Verbs { get; set; }
        protected TopicList Topics { get; set; }
        protected ItemList Items { get; set; }
        protected PersonList Persons { get; set; }
        // protected MainWindow MW
        protected LocationList Locations { get; set; }
        protected PrepList Preps { get; set; }
        protected NounList Nouns { get; set; }
        protected AdjList Adjs { get; set; }
        protected FillList Fills { get; set; }
        protected StatusList Stats { get; set; }
        protected CoBase CB{ get; set; }
        protected CoAdv CA { get; set; }
        protected List<LatestInput> LI { get; set; }

        public AbstractOrder(Adv AdvGame, AdvData A, VerbList Verbs, ItemList Items, PersonList Persons, TopicList Topics, LocationList Locations, StatusList Stats, NounList Nouns, AdjList Adjs, PrepList Preps, FillList Fills, CoBase CB, CoAdv CA, List<LatestInput> LI)
        {
            this.A = A;
            this.AdvGame = AdvGame;
            // this.MW = MW;
            this.Items = Items;
            this.Persons = Persons;
            this.Verbs = Verbs;

            this.Topics = Topics;
            this.Locations = Locations;
            this.Adjs = Adjs;
            this.Nouns = Nouns;
            this.Preps = Preps;
            this.Fills = Fills;
            this.Stats = Stats;
            // this.DoGrammar = DoGrammar;
            this.CB = CB;
            this.CA = CA;
            // Das wird hoffentlich noch obsolet mit dem nächsten Umbau
            // this.MCV = MCV;
            // this.MCE = MCE;
            this.LI = LI;
        }

        public void SetAdv(Adv AdvGame)
        {
            this.AdvGame = AdvGame;
        }

        public virtual bool Donothing(int PersoniD, ParseTokenList PTL)
        {
            return (true);
        }


        public virtual bool Examine(int PersonID, ParseTokenList PTL)
        {
            Item item = PTL.GetFirstItem(); // GetItemRef(Adv_PT[1].WordID);

            // MW.TextOutput("<br><i>Du untersuchst " + Items.GetItemNameLink(I.ID, Co.CASE_NOM) + ". (" + I.ID + ")</i>");
            AdvGame.StoryOutput( Persons.Find( PersonID ).LocationID, A.P_Everyone, "<br><i>" + Persons.GetPersonVerbLink(PersonID, Co.CASE_AKK, CB.VT_untersuchen, A.Tense) + " " + Items.GetItemNameLink(item.ID, Co.CASE_NOM) + ".</i>");

            AdvGame.StoryOutput(Persons.Find(PersonID).LocationID, PersonID, item.Description);
            if ((item.CanBeLocked) && (item.CanBeClosed))
            {
                if ((item.IsLocked) && (item.IsClosed))
                {
                    AdvGame.StoryOutput(Persons.Find(PersonID).LocationID, PersonID, Items.GetItemNameLink(item.ID, Co.CASE_AKK) + " "+Grammar.GetVerbDeclination(CB.VT_sein, A.P_3rdperson, A.Tense) + " geschlossen und abgeschlossen.");
                }
                else if ((!item.IsLocked) && (item.IsClosed))
                {
                    AdvGame.StoryOutput(Persons.Find(PersonID).LocationID, PersonID, Items.GetItemNameLink(item.ID, Co.CASE_AKK) + " " + Grammar.GetVerbDeclination(CB.VT_sein, A.P_3rdperson, A.Tense) + " geschlossen, aber nicht abgeschlossen.");
                }
                else if ((item.IsLocked) && (!item.IsClosed))
                {
                    AdvGame.StoryOutput(Persons.Find(PersonID).LocationID, PersonID, Items.GetItemNameLink(item.ID, Co.CASE_AKK) + " " + Grammar.GetVerbDeclination(CB.VT_sein, A.P_3rdperson, A.Tense) + " geöffnet, aber abgeschlossen.");
                }
            }
            else if ((!item.CanBeLocked) && (item.CanBeClosed))
            {
                if (item.IsClosed)
                {
                    AdvGame.StoryOutput(Persons.Find(PersonID).LocationID, PersonID, Items.GetItemNameLink(item.ID, Co.CASE_AKK) + " " + Grammar.GetVerbDeclination(CB.VT_sein, A.P_3rdperson, A.Tense) + " geschlossen.");
                }
                else if (!item.IsClosed)
                {
                    AdvGame.StoryOutput(Persons.Find(PersonID).LocationID, PersonID, Items.GetItemNameLink(item.ID, Co.CASE_AKK) + " " + Grammar.GetVerbDeclination(CB.VT_sein, A.P_3rdperson, A.Tense) + " geöffnet.");
                }
            }

            if ((item.CanPutIn) && (!item.InvisibleIn) && ((!item.CanBeClosed) || (item.IsClosed == false)))
            {
                Persons.ListPersons( AdvGame.CB.LocType_In_Item, item.ID, Persons.Find( PersonID ).LocationID );
                ListItems("In " + Items.GetItemNameLink(item.ID, Co.CASE_DAT) + " " + Grammar.GetVerbDeclination(CB.VT_befinden, A.P_3rdperson, A.Tense) + " sich:", PersonID, CB.LocType_In_Item, item.ID, false, true);
                item.InvisibleIn = false;
            }
            // In Cages kann man immer reingucken
            if (item.IsCage)
            {
                Persons.ListPersons(AdvGame.CB.LocType_In_Item, item.ID, Persons.Find(PersonID).LocationID);
                ListItems("In " + Items.GetItemNameLink(item.ID, Co.CASE_DAT) + " " + Grammar.GetVerbDeclination(CB.VT_befinden, A.P_3rdperson, A.Tense) + " sich:", PersonID, CB.LocType_In_Item, item.ID, false, true);
                item.InvisibleIn = false;
            }
            if ((item.CanPutOn) && (!item.InvisibleOn))
            {
                ListItems("Auf " + Items.GetItemNameLink(item.ID, Co.CASE_DAT) + " " + Grammar.GetVerbDeclination(CB.VT_befinden, A.P_3rdperson, A.Tense) + " sich:", PersonID, CB.LocType_On_Item, item.ID, false, true);
                item.InvisibleOn = false;
            }
            if ((item.CanPutBehind) && (!item.InvisibleBehind))
            {
                ListItems("Hinter " + Items.GetItemNameLink(item.ID, Co.CASE_DAT) + " " + Grammar.GetVerbDeclination(CB.VT_befinden, A.P_3rdperson, A.Tense) + " sich:", PersonID, CB.LocType_Behind_Item, item.ID, false, true);
                item.InvisibleBehind = false;
            }
            if ((item.CanPutBelow) && (!item.InvisibleBelow))
            {
                ListItems("Unter " + Items.GetItemNameLink(item.ID, Co.CASE_DAT) + " " + Grammar.GetVerbDeclination(CB.VT_befinden, A.P_3rdperson, A.Tense) + " sich:", PersonID, CB.LocType_Below_Item, item.ID, false, true);
                item.InvisibleBelow = false;
            }
            if ((item.CanPutBeside) && (!item.InvisibleBeside))
            {
                ListItems("Neben " + Items.GetItemNameLink(item.ID, Co.CASE_DAT) + " " + Grammar.GetVerbDeclination(CB.VT_befinden, A.P_3rdperson, A.Tense) + " sich:", PersonID, CB.LocType_Beside_Item, item.ID, false, true);
                item.InvisibleBeside = false;
            }
            return (true);
        }
        public virtual bool ExamineP(int PersonID, ParseTokenList PTL)
        {
            Person person = PTL.GetFirstPerson(); //  GetPersonRef(Adv_PT[1].WordID);

            AdvGame.StoryOutput(Persons.Find(PersonID).LocationID, A.P_Everyone, "<br><i>" + Persons.GetPersonVerbLink(PersonID, Co.CASE_AKK, CB.VT_untersuchen, A.Tense) + " " + Persons.GetPersonNameLink(person.ID, Co.CASE_NOM) + ".</i>");
            AdvGame.StoryOutput(Persons.Find(PersonID).LocationID, PersonID, person.Description);

            if ((person.CanBeLocked) && (person.CanBeClosed))
            {
                if ((person.IsLocked) && (person.IsClosed))
                {
                    AdvGame.StoryOutput(Persons.Find(PersonID).LocationID, PersonID, Persons.GetPersonNameLink(person.ID, Co.CASE_AKK) + " " + Grammar.GetVerbDeclination(CB.VT_sein, A.P_3rdperson, A.Tense) + " geschlossen und abgeschlossen.");
                }
                else if ((!person.IsLocked) && (person.IsClosed))
                {
                    AdvGame.StoryOutput(Persons.Find(PersonID).LocationID, PersonID, Persons.GetPersonNameLink(person.ID, Co.CASE_AKK) + " " + Grammar.GetVerbDeclination(CB.VT_sein, A.P_3rdperson, A.Tense) + " geschlossen, aber nicht abgeschlossen.");
                }
                else if ((person.IsLocked) && (!person.IsClosed))
                {
                    AdvGame.StoryOutput(Persons.Find(PersonID).LocationID, PersonID, Persons.GetPersonNameLink(person.ID, Co.CASE_AKK) + " " + Grammar.GetVerbDeclination(CB.VT_sein, A.P_3rdperson, A.Tense) + " geöffnet, aber abgeschlossen.");
                }
            }
            else if ((!person.CanBeLocked) && (person.CanBeClosed))
            {
                if (person.IsClosed)
                {
                    AdvGame.StoryOutput(Persons.Find(PersonID).LocationID, PersonID, Persons.GetPersonNameLink(person.ID, Co.CASE_AKK) + " " + Grammar.GetVerbDeclination(CB.VT_sein, A.P_3rdperson, A.Tense) + " geschlossen.");
                }
                else if (!person.IsClosed)
                {
                    AdvGame.StoryOutput(Persons.Find(PersonID).LocationID, PersonID, Persons.GetPersonNameLink(person.ID, Co.CASE_AKK) + " " + Grammar.GetVerbDeclination(CB.VT_sein, A.P_3rdperson, A.Tense) + " geöffnet.");
                }
            }

            if ((person.CanPutIn) && ((!person.CanBeClosed) || (person.IsClosed == false)))
            {
                ListItems("In " + Persons.GetPersonNameLink(person.ID, Co.CASE_DAT) + " " + Grammar.GetVerbDeclination(CB.VT_befinden, A.P_3rdperson, A.Tense) + " sich:", PersonID, CB.LocType_In_Person, person.ID, false, true);
            }

            return (true);
        }

        public virtual bool Take(int PersonID, ParseTokenList PTL)
        {
            // Nach den gesamten Prüfungen von Adv_PT ist garantiert, dass sich die Werte an dieser Stelle befinden.
            Item item  = PTL.GetFirstItem(); //  GetItemRef(Adv_PT[1].WordID);

            return (ProcessTake(item, PersonID ));


        }
        public virtual bool TakeP(int PersonID, ParseTokenList PTL)
        {
            // Nach den gesamten Prüfungen von Adv_PT ist garantiert, dass sich die Werte an dieser Stelle befinden.
            Person pers  = PTL.GetFirstPerson(); //  GetPersonRef(Adv_PT[1].WordID);

            return (ProcessTakeP(pers, PersonID ));

        }
        public virtual bool Drop(int PersonID, ParseTokenList PTL)
        {
            // Nach den gesamten Prüfungen von Adv_PT ist garantiert, dass sich die Werte an dieser Stelle befinden.
            Item item = PTL.GetFirstItem(); //  GetItemRef(Adv_PT[1].WordID);

            if ((item.LocationType == CB.LocType_Person) && (item.LocationID == PersonID ))
            {
                AdvGame.StoryOutput(Persons.Find(PersonID).LocationID, A.P_Everyone, "<br><i>" + Persons.GetPersonVerbLink(PersonID, Co.CASE_AKK, CB.VT_legen, A.Tense) + " " + Items.GetItemNameLink(item.ID, Co.CASE_NOM) + " ab. (" + item.ID + ")</i>");
                Items.TransferItem(item, CB.LocType_Loc, Persons.Find(PersonID).LocationID);
            }
            else
            {
                AdvGame.StoryOutput(Persons.Find(PersonID).LocationID, A.P_Everyone, "<br>So etwas " + Grammar.GetVerbDeclination(CB.VT_haben, PersonID, A.Tense) + " " + Persons.GetPersonNameLink(PersonID, Co.CASE_AKK) + " nicht dabei.");
            }
            return (true);
        }

        public virtual bool Inventory(int PersonID, ParseTokenList PTL)
        {
            AdvGame.StoryOutput(Persons.Find(PersonID).LocationID, A.P_Everyone, "<br><i>" + Persons.GetPersonVerbLink(PersonID, Co.CASE_AKK, CB.VT_schauen, A.Tense) + " nach, was " + Grammar.GetPronoun(PersonID) + " dabei " + Grammar.GetVerbDeclination(CB.VT_haben, PersonID, A.Tense) + ".</i>");

            ListItems(Persons.GetPersonVerbLink(PersonID, Co.CASE_AKK, CB.VT_sehen, A.Tense) + " ", PersonID, CB.LocType_Person, PersonID, false, false);

            return (true);
        }
        public virtual bool Location(int PersonID, ParseTokenList PTL)
        {

            AdvGame.StoryOutput(Persons.Find(PersonID).LocationID, A.P_Everyone, "<br><i>" + Persons.GetPersonVerbLink(PersonID, Co.CASE_AKK, CB.VT_schauen, A.Tense) + " " + Grammar.GetReflexivePronoun(PersonID, Co.CASE_AKK) + " um.</i>");
            Locations.ShowLocationFull(Persons.LocOnly(PersonID) );

            return (true);
        }
        public virtual bool Go(int PersonID, int Dir)
        {
            if (Locations.LocPersonAct(PersonID).LocExit[Dir] > 0)
            {
                AdvGame.StoryOutput( Persons.Find(PersonID).LocationID, A.P_Everyone, "<br><i>" + Persons.GetPersonVerbLink(PersonID, Co.CASE_AKK, CB.VT_gehen, A.Tense) + " nach " + Locations.GetDirection(Dir) + ".</i>");

                Persons.TransferPerson(PersonID, CB.LocType_Loc, Locations.LocPersonAct(PersonID).LocExit[Dir]);

                // Persons.Find(PersonID).LocationID = Locations.LocPersonAct(PersonID).LocExit[Dir];
                // Persons.Find(PersonID).LocationType = CB.LocType_Loc;
                
                if (PersonID == A.ActPerson)
                {
                    A.ActLoc = Persons.Find(PersonID).LocationID;
                    Locations.ShowLocation(A.ActLoc);
                }
                if( ( PersonID != A.ActPerson) && (Persons.Find(PersonID).LocationID == Persons.Find( A.ActPerson).LocationID ) )
                {
                    AdvGame.StoryOutput(Persons.Find(PersonID).LocationID, A.P_Everyone, "<br><i>" + Persons.GetPersonVerbLink(PersonID, Co.CASE_AKK, CB.VT_erscheinen, A.Tense) + ".</i>");

                }
            }
            else
                AdvGame.FeedbackOutput(PersonID, "Dahin führt leider kein Weg.");
            return (true);
        }
        public virtual bool GoN(int PersonID, ParseTokenList PTL)
        {
            return (Go(PersonID, Co.DIR_N));
        }
        public virtual bool GoNE(int PersonID, ParseTokenList PTL)
        {
            return (Go(PersonID, Co.DIR_NE));
        }
        public virtual bool GoE(int PersonID,ParseTokenList PTL)
        {
            return (Go(PersonID, Co.DIR_E));
        }
        public virtual bool GoSE(int PersonID,ParseTokenList PTL)
        {
            return (Go(PersonID, Co.DIR_SE));
        }
        public virtual bool GoS(int PersoniD, ParseTokenList PTL)
        {
            return (Go(PersoniD, Co.DIR_S));
        }
        public virtual bool GoSW(int PersoniD, ParseTokenList PTL)
        {
            return (Go(PersoniD, Co.DIR_SW));
        }
        public virtual bool GoW(int PersoniD, ParseTokenList PTL)
        {
            return (Go(PersoniD, Co.DIR_W));
        }
        public virtual bool GoNW(int PersoniD, ParseTokenList PTL)
        {
            return (Go(PersoniD, Co.DIR_NW));
        }
        public virtual bool GoU(int PersoniD, ParseTokenList PTL)
        {
            return (Go(PersoniD, Co.DIR_U));
        }
        public virtual bool GoD(int PersoniD, ParseTokenList PTL)
        {
            return (Go(PersoniD, Co.DIR_D));
        }
        public virtual bool Open(int PersonID,  ParseTokenList PTL)
        {
            bool succeed = true;

            Item item = PTL.GetFirstItem(); //  GetItemRef(Adv_PT[1].WordID);
                                         // int ItemID = GetItemIx(Adv_PT[1].WordID);
            if (item.CanBeClosed == false)
            {
                AdvGame.FeedbackOutput(PersonID, Items.GetItemNameLink(item.ID, Co.CASE_AKK) + " kann weder geöffnet noch geschlossen werden.");
                succeed = false;
            }
            else if ((item.CanBeClosed == true) && (item.CanBeLocked == true) && (item.IsLocked == true))
            {
                AdvGame.FeedbackOutput(PersonID, Items.GetItemNameLink(item.ID, Co.CASE_AKK) + " " + Grammar.GetVerbDeclination(CB.VT_sein, A.P_3rdperson, A.Tense) + " abgeschlossen.");
                succeed = false;
            }
            else if (item.IsClosed == false)
            {
                AdvGame.FeedbackOutput(PersonID, Items.GetItemNameLink(item.ID, Co.CASE_AKK) + " " + Grammar.GetVerbDeclination(CB.VT_sein, A.P_3rdperson, A.Tense) + " bereits offen.");
            }
            else
            {
                AdvGame.StoryOutput(Persons.Find(PersonID).LocationID, A.P_Everyone, "<br><i>" + Persons.GetPersonVerbLink(PersonID, Co.CASE_AKK, CB.VT_oeffnen, A.Tense) + " " + Items.GetItemNameLink(item.ID, Co.CASE_NOM) + ".</i>");
                item.IsClosed = false;
            }
            return (succeed);
        }
        public virtual bool Close(int PersonID, ParseTokenList PTL)
        {
            Item item = PTL.GetFirstItem(); //  GetItemRef(Adv_PT[1].WordID);
                                         // int ItemID = GetItemIx(Adv_PT[1].WordID);
            if (item.CanBeClosed == false)
            {
                AdvGame.FeedbackOutput(PersonID, Items.GetItemNameLink(item.ID, Co.CASE_AKK) + " kann weder geöffnet noch geschlossen werden.");
            }
            else if (item.IsClosed == true)
            {
                AdvGame.FeedbackOutput(PersonID, Items.GetItemNameLink(item.ID, Co.CASE_AKK) + " " + Grammar.GetVerbDeclination(CB.VT_sein, A.P_3rdperson, A.Tense) + " bereits geschlossen.");
            }
            else
            {
                AdvGame.StoryOutput(Persons.Find(PersonID).LocationID, A.P_Everyone, "<br><i>" + Persons.GetPersonVerbLink(PersonID, Co.CASE_AKK, CB.VT_schliessen, A.Tense) + " " + Items.GetItemNameLink(item.ID, Co.CASE_NOM) + ".</i>");
                item.IsClosed = true;
            }
            return (true);
        }
        public virtual bool OpenP(int PersonID, ParseTokenList PTL)
        {
            bool succeed = true;

            Person person = PTL.GetFirstPerson(); //  GetPersonRef(Adv_PT[1].WordID);
            if (person.CanBeClosed == false)
            {
                AdvGame.FeedbackOutput(PersonID, Persons.GetPersonNameLink(person.ID, Co.CASE_AKK) + " kann weder geöffnet noch geschlossen werden.");
                succeed = false;
            }
            else if ((person.CanBeClosed == true) && (person.CanBeLocked == true) && (person.IsLocked == true))
            {
                AdvGame.FeedbackOutput(PersonID, Persons.GetPersonNameLink(person.ID, Co.CASE_AKK) + " " + Grammar.GetVerbDeclination(CB.VT_sein, A.P_3rdperson, A.Tense) + " abgeschlossen.");
                succeed = false;
            }
            else if (person.IsClosed == false)
            {
                AdvGame.FeedbackOutput(PersonID, Persons.GetPersonNameLink(person.ID, Co.CASE_AKK) + " " + Grammar.GetVerbDeclination(CB.VT_sein, A.P_3rdperson, A.Tense) + " bereits offen.");
            }
            else
            {
                AdvGame.StoryOutput(Persons.Find(PersonID).LocationID, A.P_Everyone, "<br><i>" + Persons.GetPersonVerbLink(PersonID, Co.CASE_AKK, CB.VT_oeffnen, A.Tense) + " " + Persons.GetPersonNameLink(person.ID, Co.CASE_NOM) + ".</i>");
                person.IsClosed = false;
            }
            return (succeed);
        }
        public virtual bool CloseP(int PersonID, ParseTokenList PTL)
        {
            Person person = PTL.GetFirstPerson(); //  GetPersonRef(Adv_PT[1].WordID);
                                             // int ItemID = GetItemIx(Adv_PT[1].WordID);
            if (person.CanBeClosed == false)
            {
                AdvGame.FeedbackOutput(PersonID, Persons.GetPersonNameLink(person.ID, Co.CASE_AKK) + " kann weder geöffnet noch geschlossen werden.");
            }
            else if (person.IsClosed == true)
            {
                AdvGame.FeedbackOutput(PersonID, Persons.GetPersonNameLink(person.ID, Co.CASE_AKK) + " " + Grammar.GetVerbDeclination(CB.VT_sein, A.P_3rdperson, A.Tense) + " bereits geschlossen.");
            }
            else
            {
                AdvGame.StoryOutput(Persons.Find(PersonID).LocationID, A.P_Everyone, "<br><i>" + Persons.GetPersonVerbLink(PersonID, Co.CASE_AKK, CB.VT_schliessen, A.Tense) + " " + Persons.GetPersonNameLink(person.ID, Co.CASE_NOM) + ".</i>");
                person.IsClosed = true;
            }
            return (true);
        }
        public virtual bool UnlockW(int PersonID, ParseTokenList PTL)
        {
            Item item1 = PTL.GetFirstItem(); //  GetItemRef(Adv_PT[1].WordID);
            Item item2 = PTL.GetSecondItem(); //  GetItemRef(Adv_PT[3].WordID);
                                           // int ItemID1 = GetItemIx(Adv_PT[1].WordID);
                                           // int ItemID2 = GetItemIx(Adv_PT[3].WordID);

            if (item1.CanBeLocked == false)
            {
                AdvGame.FeedbackOutput(PersonID, Items.GetItemNameLink(item1.ID, Co.CASE_AKK) + " " + Grammar.GetVerbDeclination(CB.VT_sein, A.P_3rdperson, A.Tense) + " nicht abschließbar.");
            }
            else if (item1.IsLocked == false)
            {
                AdvGame.FeedbackOutput(PersonID, Items.GetItemNameLink(item1.ID, Co.CASE_AKK) + " " + Grammar.GetVerbDeclination(CB.VT_sein, A.P_3rdperson, A.Tense) + " schon aufgeschlossen.");
            }
            else
            {
                bool found = false;

                foreach (int i in item1.UnlockItems)
                {
                    if (i == item2.ID)
                    {
                        item1.IsLocked = false;
                        AdvGame.StoryOutput(Persons.Find(PersonID).LocationID, A.P_Everyone, "<br><i>" + Persons.GetPersonVerbLink(PersonID, Co.CASE_AKK, CB.VT_schliessen, A.Tense) + " " + Items.GetItemNameLink(item1.ID, Co.CASE_NOM) + " mit " + Items.GetItemNameLink(item2.ID, Co.CASE_DAT) + " auf.</i>");

                        found = true;
                        break;
                    }
                }
                if (found == false)
                {
                    AdvGame.StoryOutput(Persons.Find(PersonID).LocationID, A.P_Everyone, Items.GetItemNameLink(item1.ID, Co.CASE_AKK) + " lässt sich nicht mit " + Items.GetItemNameLink(item2.ID, Co.CASE_DAT) + " aufschließen.");
                }
            }
            return (true);
        }
        public virtual bool UnlockWP(int PersonID, ParseTokenList PTL)
        {
            Person person1 = PTL.GetFirstPerson(); //  GetPersonRef(Adv_PT[1].WordID);
            Item item2 = PTL.GetFirstItem(); //  GetItemRef(Adv_PT[3].WordID);

            if (person1.CanBeLocked == false)
            {
                AdvGame.FeedbackOutput(PersonID, Persons.GetPersonNameLink(person1.ID, Co.CASE_AKK) + " kann man nicht auf- oder zuschließen.");
            }
            else if (person1.IsLocked == false)
            {
                AdvGame.FeedbackOutput(PersonID, Persons.GetPersonNameLink(person1.ID, Co.CASE_AKK) + " " + Grammar.GetVerbDeclination(CB.VT_sein, A.P_3rdperson, A.Tense) + " schon aufgeschlossen.");
            }
            else
            {
                bool found = false;

                foreach (int i in person1.UnlockItems)
                {
                    if (i == item2.ID)
                    {
                        person1.IsLocked = false;
                        AdvGame.StoryOutput(Persons.Find(PersonID).LocationID, A.P_Everyone, "<br><i>" + Persons.GetPersonVerbLink(PersonID, Co.CASE_AKK, CB.VT_schliessen, A.Tense) + " " + Persons.GetPersonNameLink(person1.ID, Co.CASE_NOM) + " mit " + Items.GetItemNameLink(item2.ID, Co.CASE_DAT) + " auf.</i>");

                        found = true;
                        break;
                    }
                }
                if (found == false)
                {
                    AdvGame.StoryOutput(Persons.Find(PersonID).LocationID, A.P_Everyone, Persons.GetPersonNameLink(person1.ID, Co.CASE_AKK) + " lässt sich nicht mit " + Items.GetItemNameLink(item2.ID, Co.CASE_DAT) + " aufschließen.");
                }
            }
            return (true);
        }
        public virtual bool LockW(int PersonID, ParseTokenList PTL)
        {
            Item item1 = PTL.GetFirstItem(); //  GetItemRef(Adv_PT[1].WordID);
            Item item2 = PTL.GetSecondItem(); //  GetItemRef(Adv_PT[3].WordID);
                                           // int ItemID1 = GetItemIx(Adv_PT[1].WordID);
                                           // int ItemID2 = GetItemIx(Adv_PT[3].WordID);

            if (item1.CanBeLocked == false)
            {
                AdvGame.FeedbackOutput(PersonID, Items.GetItemNameLink(item1.ID, Co.CASE_AKK) + " " + Grammar.GetVerbDeclination(CB.VT_sein, A.P_3rdperson, A.Tense) + " nicht abschließbar.");
            }
            else if (item1.IsLocked == true)
            {
                AdvGame.FeedbackOutput(PersonID, Items.GetItemNameLink(item1.ID, Co.CASE_AKK) + " " + Grammar.GetVerbDeclination(CB.VT_sein, A.P_3rdperson, A.Tense) + " schon abgeschlossen.");
            }
            else
            {
                bool found = false;

                foreach (int i in item1.UnlockItems)
                {
                    if (i == item2.ID)
                    {
                        item1.IsLocked = true;
                        AdvGame.StoryOutput(Persons.Find(PersonID).LocationID, A.P_Everyone, "<br><i>" + Persons.GetPersonVerbLink(PersonID, Co.CASE_AKK, CB.VT_schliessen, A.Tense) + " " + Items.GetItemNameLink(item1.ID, Co.CASE_NOM) + " mit " + Items.GetItemNameLink(item2.ID, Co.CASE_DAT) + " ab.</i>");

                        found = true;
                        break;
                    }
                }
                if (found == false)
                {
                    AdvGame.StoryOutput(Persons.Find(PersonID).LocationID, A.P_Everyone, Items.GetItemNameLink(item1.ID, Co.CASE_AKK) + " lässt sich nicht mit " + Items.GetItemNameLink(item2.ID, Co.CASE_DAT) + " abschließen.");
                }
            }
            return (true);
        }
        public virtual bool LockWP(int PersonID, ParseTokenList PTL)
        {
            Person person1 = PTL.GetFirstPerson(); //  GetPersonRef(Adv_PT[1].WordID);
            Item item2 = PTL.GetFirstItem(); //  GetItemRef(Adv_PT[3].WordID);
                                          // int ItemID1 = GetItemIx(Adv_PT[1].WordID);
                                          // int ItemID2 = GetItemIx(Adv_PT[3].WordID);

            if (person1.CanBeLocked == false)
            {
                AdvGame.FeedbackOutput(PersonID, Persons.GetPersonNameLink(person1.ID, Co.CASE_AKK) + " " + Grammar.GetVerbDeclination(CB.VT_sein, A.P_3rdperson, A.Tense) + " nicht abschließbar.");
            }
            else if (person1.IsLocked == true)
            {
                AdvGame.FeedbackOutput(PersonID, Persons.GetPersonNameLink(person1.ID, Co.CASE_AKK) + " " + Grammar.GetVerbDeclination(CB.VT_sein, A.P_3rdperson, A.Tense) + " schon abgeschlossen.");
            }
            else
            {
                bool found = false;

                foreach (int i in person1.UnlockItems)
                {
                    if (i == item2.ID)
                    {
                        person1.IsLocked = true;
                        AdvGame.StoryOutput(Persons.Find(PersonID).LocationID, A.P_Everyone, "<br><i>" + Persons.GetPersonVerbLink(PersonID, Co.CASE_AKK, CB.VT_schliessen, A.Tense) + " " + Persons.GetPersonNameLink(person1.ID, Co.CASE_NOM) + " mit " + Items.GetItemNameLink(item2.ID, Co.CASE_DAT) + " ab.</i>");

                        found = true;
                        break;
                    }
                }
                if (found == false)
                {
                    AdvGame.StoryOutput(Persons.Find(PersonID).LocationID, A.P_Everyone, Persons.GetPersonNameLink(person1.ID, Co.CASE_AKK) + " lässt sich nicht mit " + Items.GetItemNameLink(item2.ID, Co.CASE_DAT) + " abschließen.");
                }
            }
            return (true);
        }
        public virtual bool TakeUnder(int PersonID, ParseTokenList PTL)
        {
            return (Take(PersonID, PTL));
        }
        public virtual bool TakeOut(int PersonID, ParseTokenList PTL)
        {
            return (Take(PersonID, PTL));
        }
        public virtual bool TakeFrom(int PersonID, ParseTokenList PTL)
        {
            return (Take(PersonID, PTL));
        }
        public virtual bool TakeBehind(int PersonID, ParseTokenList PTL)
        {
            return (Take(PersonID, PTL));
        }
        public virtual bool TakeBeside(int PersonID, ParseTokenList PTL)
        {
            return (Take(PersonID, PTL));
        }
        public virtual bool PlaceUnder(int PersonID, ParseTokenList PTL)
        {
            Item item1 = PTL.GetFirstItem(); //  GetItemRef(Adv_PT[1].WordID);
            Item item2 = PTL.GetSecondItem(); //  GetItemRef(Adv_PT[3].WordID);

            if (item2.CanPutBelow)
            {
                if (item1.Size > item2.StorageBelow)
                {
                    AdvGame.FeedbackOutput(PersonID, Items.GetItemNameLink(item1.ID, Co.CASE_AKK) + " passte nicht unter " + Items.GetItemNameLink(item2.ID, Co.CASE_NOM) + ".");
                }
                else if (item1 == item2 )
                {
                    AdvGame.FeedbackOutput(PersonID, Items.GetItemNameLink(item1.ID, Co.CASE_NOM) + " unter" + Items.GetItemNameLink(item2.ID, Co.CASE_NOM) + " legen? Ganz schlechte Idee.");
                }
                else
                {
                    AdvGame.StoryOutput(Persons.Find(PersonID).LocationID, A.P_Everyone, "<br><i>" + Persons.GetPersonVerbLink(PersonID, Co.CASE_AKK, CB.VT_legen, A.Tense) + " " + Items.GetItemNameLink(item1.ID, Co.CASE_AKK) + " unter " + Items.GetItemNameLink(item2.ID, Co.CASE_DAT) + ".</i>");
                    Items.TransferItem(item1, CB.LocType_Below_Item, item2.ID);
                }
            }
            else
                AdvGame.FeedbackOutput(PersonID, "Unter " + Items.GetItemNameLink(item2.ID, Co.CASE_NOM) + " kann man nichts drunterlegen.");

            return (true);
        }
        public virtual bool PlaceIn(int PersonID, ParseTokenList PTL)
        {
            Item item1 = PTL.GetFirstItem(); //  GetItemRef(Adv_PT[1].WordID);
            Item item2 = PTL.GetSecondItem(); //  GetItemRef(Adv_PT[3].WordID);

            if (item1 == item2)
            {
                AdvGame.FeedbackOutput(PersonID, Items.GetItemNameLink(item1.ID, Co.CASE_NOM) + " in " + Items.GetItemNameLink(item2.ID, Co.CASE_NOM) + " legen? Ganz schlechte Idee.");
            }
            else if (item2.CanPutIn)
            {
                bool AutoOpen = false;

                // Wenn ein Item geschlossen ist, wird versucht, es zu öffnen
                if ((item2.IsClosed) && (item2.CanBeClosed))
                {
                    ParseTokenList PT = new ParseTokenList(Verbs, Preps, Nouns, Adjs, Fills);
                    PT.AddVerb((int)CB.VerbID_Open);
                    PT.AddItem(item2);
                    if (Open(PersonID, PT)) AutoOpen = true;
                    // Wenn sich das Item nicht öffnen ließ, dann wurde hier ein Kommentar ausgegeben und diese Funktion muss nichts mehr schreiben.
                }
                if ((!item2.IsClosed) && (item2.CanBeClosed))
                {
                    if (item1.Size > item2.StorageIn)
                    {
                        AdvGame.FeedbackOutput(PersonID, Items.GetItemNameLink(item1.ID, Co.CASE_AKK) + " passte nicht in " + Items.GetItemNameLink(item2.ID, Co.CASE_NOM) + ".");
                    }
                    else
                    {
                        AdvGame.StoryOutput(Persons.Find(PersonID).LocationID, A.P_Everyone, "<br><i>" + Persons.GetPersonVerbLink(PersonID, Co.CASE_AKK, CB.VT_legen, A.Tense) + " " + Items.GetItemNameLink(item1.ID, Co.CASE_AKK) + " in " + Items.GetItemNameLink(item2.ID, Co.CASE_DAT) + ".</i>");
                        Items.TransferItem(item1, CB.LocType_In_Item, item2.ID);
                    }
                }
                if (AutoOpen)
                {
                    ParseTokenList PT = new ParseTokenList(Verbs, Preps, Nouns, Adjs, Fills);
                    PT.AddVerb((int)CB.VerbID_Open);
                    PT.AddItem(item2);
                    Close(PersonID, PT);
                    AutoOpen = true;
                    // Wenn sich das Item nicht öffnen ließ, dann wurde hier ein Kommentar ausgegeben und diese Funktion muss nichts mehr schreiben.
                }
            }
            else
                AdvGame.FeedbackOutput(PersonID, "In " + Items.GetItemNameLink(item2.ID, Co.CASE_NOM) + " kann man nichts hineinlegen.");

            return (true);
        }
        public virtual bool PlaceInP(int PersonID, ParseTokenList PTL)
        {
            Item item1 = PTL.GetFirstItem(); //  GetItemRef(Adv_PT[1].WordID);
            Person person2 = PTL.GetFirstPerson(); //  GetPersonRef(Adv_PT[3].WordID);

            if (person2.CanPutIn)
            {
                bool autoOpen = false;

                // Wenn ein Item geschlossen ist, wird versucht, es zu öffnen
                if ((person2.IsClosed) && (person2.CanBeClosed))
                {
                    ParseTokenList PT = new ParseTokenList(Verbs, Preps, Nouns, Adjs, Fills);
                    PT.AddVerb((int)CB.VerbID_Open);
                    PT.AddPerson(person2);
                    if (Open(PersonID, PT)) 
                        autoOpen = true;
                    // Wenn sich das Item nicht öffnen ließ, dann wurde hier ein Kommentar ausgegeben und diese Funktion muss nichts mehr schreiben.
                }
                if ((!person2.IsClosed) && (person2.CanBeClosed))
                {
                    if (item1.Size > person2.StorageIn)
                    {
                        AdvGame.FeedbackOutput(PersonID, Items.GetItemNameLink(item1.ID, Co.CASE_AKK) + " passt nicht in " + Persons.GetPersonNameLink(person2.ID, Co.CASE_NOM) + ".");
                    }
                    else
                    {
                        AdvGame.StoryOutput(Persons.Find(PersonID).LocationID, A.P_Everyone, "<br><i>" + Persons.GetPersonVerbLink(PersonID, Co.CASE_AKK, CB.VT_legen, A.Tense) + " " + Items.GetItemNameLink(item1.ID, Co.CASE_AKK) + " in " + Persons.GetPersonNameLink(person2.ID, Co.CASE_DAT) + ".</i>");
                        Items.TransferItem(item1, CB.LocType_In_Person, person2.ID);
                    }
                }
                if (autoOpen)
                {
                    ParseTokenList PT = new ParseTokenList(Verbs, Preps, Nouns, Adjs, Fills);
                    PT.AddVerb((int)CB.VerbID_Open);
                    PT.AddPerson(person2);
                    Close(PersonID, PT);
                    autoOpen = true;
                    // Wenn sich das Item nicht öffnen ließ, dann wurde hier ein Kommentar ausgegeben und diese Funktion muss nichts mehr schreiben.
                }
            }
            else
                AdvGame.FeedbackOutput(PersonID, "In " + Persons.GetPersonNameLink(person2.ID, Co.CASE_NOM) + " kann man nichts hineinlegen.");

            return (true);
        }
        public virtual bool GiveToP(int PersonID, ParseTokenList PTL)
        {
            Item item1 = PTL.GetFirstItem(); //  GetItemRef(Adv_PT[1].WordID);
            Person person2 = PTL.GetFirstPerson(); //  GetPersonRef(Adv_PT[3].WordID);

            if (item1.Size > person2.StorageIn)
            {
                AdvGame.FeedbackOutput(PersonID, Persons.GetPersonNameLink(person2.ID, Co.CASE_DAT) + " konnte " + Items.GetItemNameLink(item1.ID, Co.CASE_AKK) + " nicht mehr geben.");
            }
            else
            {
                AdvGame.StoryOutput(Persons.Find(PersonID).LocationID, A.P_Everyone, "<br><i>" + Persons.GetPersonVerbLink(PersonID, Co.CASE_AKK, CB.VT_geben, A.Tense) + " " + Persons.GetPersonNameLink(item1.ID, Co.CASE_AKK) + " in " + Persons.GetPersonNameLink(person2.ID, Co.CASE_DAT) + ".</i>");
                Items.TransferItem(item1, CB.LocType_To_Person, person2.ID);
            }
            return (true);
        }
        public virtual bool PlaceOn(int PersonID, ParseTokenList PTL)
        {
            Item item1 = PTL.GetFirstItem(); //  GetItemRef(Adv_PT[1].WordID);
            Item item2 = PTL.GetSecondItem(); //  GetItemRef(Adv_PT[3].WordID);

            if (item2.CanPutOn)
            {
                if (item1.Size > item2.StorageOn)
                {
                    AdvGame.FeedbackOutput(PersonID, Items.GetItemNameLink(item1.ID, Co.CASE_AKK) + " passte nicht auf " + Items.GetItemNameLink(item2.ID, Co.CASE_NOM) + ".");
                }
                else if (item1 == item2)
                {
                    AdvGame.FeedbackOutput(PersonID, Items.GetItemNameLink(item1.ID, Co.CASE_NOM) + " auf " + Items.GetItemNameLink(item2.ID, Co.CASE_NOM) + " legen? Ganz schlechte Idee.");
                }
                else
                {
                    AdvGame.StoryOutput(Persons.Find(PersonID).LocationID, A.P_Everyone, "<br><i>" + Persons.GetPersonVerbLink(PersonID, Co.CASE_AKK, CB.VT_legen, A.Tense) + " " + Items.GetItemNameLink(item1.ID, Co.CASE_AKK) + " auf " + Items.GetItemNameLink(item2.ID, Co.CASE_DAT) + ".</i>");
                    Items.TransferItem(item1, CB.LocType_On_Item, item2.ID);
                }
            }
            else
                AdvGame.FeedbackOutput(PersonID, "Auf " + Items.GetItemNameLink(item2.ID, Co.CASE_NOM) + " kann man nichts drauflegen.");

            return (true);
        }
        public virtual bool PlaceBehind(int PersonID, ParseTokenList PTL)
        {
            Item item1 = PTL.GetFirstItem(); //  GetItemRef(Adv_PT[1].WordID);
            Item item2 = PTL.GetSecondItem(); //  GetItemRef(Adv_PT[3].WordID);

            if (item2.CanPutBehind)
            {
                if (item1.Size > item2.StorageBehind)
                {
                    AdvGame.FeedbackOutput(PersonID, Items.GetItemNameLink(item1.ID, Co.CASE_AKK) + " passte nicht hinter " + Items.GetItemNameLink(item2.ID, Co.CASE_NOM) + ".");
                }
                else if (item1 == item2)
                {
                    AdvGame.FeedbackOutput(PersonID, Items.GetItemNameLink(item1.ID, Co.CASE_NOM) + " hinter " + Items.GetItemNameLink(item2.ID, Co.CASE_NOM) + " legen? Ganz schlechte Idee.");
                }
                else
                {
                    AdvGame.StoryOutput(Persons.Find(PersonID).LocationID, A.P_Everyone, "<br><i>" + Persons.GetPersonVerbLink(PersonID, Co.CASE_AKK, CB.VT_legen, A.Tense) + " " + Items.GetItemNameLink(item1.ID, Co.CASE_AKK) + " hinter " + Items.GetItemNameLink(item2.ID, Co.CASE_DAT) + " ab.</i>");
                    Items.TransferItem(item1, CB.LocType_Behind_Item, item2.ID);
                }
            }
            else
                AdvGame.FeedbackOutput(PersonID, "Hinter " + Items.GetItemNameLink(item2.ID, Co.CASE_NOM) + " kann man nichts ablegen.");

            return (true);
        }
        public virtual bool PlaceBeside(int PersonID, ParseTokenList PTL)
        {
            Item item1 = PTL.GetFirstItem(); //  GetItemRef(Adv_PT[1].WordID);
            Item item2 = PTL.GetSecondItem(); //  GetItemRef(Adv_PT[3].WordID);

            if (item2.CanPutBeside)
            {
                if (item1.Size > item2.StorageBeside)
                {
                    AdvGame.FeedbackOutput(PersonID, Items.GetItemNameLink(item1.ID, Co.CASE_AKK) + " passte nicht neben " + Items.GetItemNameLink(item2.ID, Co.CASE_NOM) + ".");
                }
                else if (item1 == item2)
                {
                    AdvGame.FeedbackOutput(PersonID, Items.GetItemNameLink(item1.ID, Co.CASE_NOM) + " neben " + Items.GetItemNameLink(item2.ID, Co.CASE_NOM) + " legen? Ganz schlechte Idee.");
                }
                else
                {
                    AdvGame.StoryOutput(Persons.Find(PersonID).LocationID, A.P_Everyone, "<br><i>" + Persons.GetPersonVerbLink(PersonID, Co.CASE_AKK, CB.VT_legen, A.Tense) + " " + Items.GetItemNameLink(item1.ID, Co.CASE_AKK) + " neben " + Items.GetItemNameLink(item2.ID, Co.CASE_DAT) + " ab.</i>");
                    Items.TransferItem(item1, CB.LocType_Beside_Item, item2.ID);
                }
            }
            else
                AdvGame.FeedbackOutput(PersonID, "Neben " + Items.GetItemNameLink(item2.ID, Co.CASE_NOM) + " kann man nichts ablegen.");

            return (true);
        }
        public virtual bool ExamineBelow(int PersonID, ParseTokenList PTL)
        {
            Item item1 = PTL.GetFirstItem(); //  GetItemRef(Adv_PT[2].WordID);
            AdvGame.StoryOutput(Persons.Find(PersonID).LocationID, A.P_Everyone, "<br><i>" + Persons.GetPersonVerbLink(PersonID, Co.CASE_AKK, CB.VT_schauen, A.Tense) + " unter " + Items.GetItemNameLink(item1.ID, Co.CASE_DAT) + ".</i>");
            ListItems(Persons.GetPersonVerbLink(PersonID, Co.CASE_AKK, CB.VT_sehen, A.Tense) + ": ", PersonID, CB.LocType_Below_Item, item1.ID, true, false);
            return (true);

        }
        public virtual bool Taste(int PersonID, ParseTokenList PTL)
        {
            Item item1 = PTL.GetFirstItem(); //  GetItemRef(Adv_PT[2].WordID);
            AdvGame.StoryOutput(Persons.Find(PersonID).LocationID, A.P_Everyone, "<br><i>" + Persons.GetPersonVerbLink(PersonID, Co.CASE_AKK, CB.VT_schmecken, A.Tense) + " an " + Items.GetItemNameLink(item1.ID, Co.CASE_DAT) + ". (" + item1.ID + ")</i>");
            return (true);
        }
        public virtual bool Smell(int PersonID, ParseTokenList PTL)
        {
            Item item1 = PTL.GetFirstItem(); //  GetItemRef(Adv_PT[2].WordID);
            AdvGame.StoryOutput(Persons.Find(PersonID).LocationID, A.P_Everyone, "<br><i>" + Persons.GetPersonVerbLink(PersonID, Co.CASE_AKK, CB.VT_riechen, A.Tense) + " an " + Items.GetItemNameLink(item1.ID, Co.CASE_DAT) + ". (" + item1.ID + ")</i>");
            return (true);
        }
        public virtual bool Wait(int PersonID, ParseTokenList PTL)
        {
            Item item1 = PTL.GetFirstItem(); //  GetItemRef(Adv_PT[2].WordID);
            AdvGame.StoryOutput(Persons.Find(PersonID).LocationID, A.P_Everyone, "<br><i>" + Persons.GetPersonVerbLink(PersonID, Co.CASE_AKK, CB.VT_warten, A.Tense) + ".</i>");
            return (true);
        }

        public bool DoQuit(List<MCMenuEntry> MCMEntry)
        {
            AdvGame.Autosave();
            AdvGame.A.Finish = true;
            AdvGame.Close();
            return (true);
        }
        public virtual bool Quit(int PersonID, ParseTokenList PTL)
        {
            MCMenu mcM = new MCMenu(CB, Stats, Persons, A.P_Self, A, AdvGame,  true, 1 + CB.MCE_Choice_Off);
            List<int> cFollower;
            cFollower = new List<int>();

            // 1 
            cFollower.Add(1);
            mcM.Add(new MCMenuEntry(0, "Willst du wirklich aufhören?", 1, 0, false));

            // 2 
            cFollower.Add(2);
            mcM.Add(new MCMenuEntry(A.P_Self, "Ja", 2, -1, false));
            mcM.Last().Del = DoQuit;

            // 3 
            cFollower.Add(3);
            mcM.Add(new MCMenuEntry(A.P_Self, "Nein", 3, -1, false));

            mcM.Add(new MCMenuEntry(1 + CB.MCE_Choice_Off, cFollower));

            mcM.MCS = mcM.MenuShow();
            mcM.Set(0);
            mcM.MCS.MCOutput(mcM, mcM.MCSelection, false);


            return (true);
        }
        public virtual bool Restart(int PersonID, ParseTokenList PTL)
        {
            MCMenu mcM = new MCMenu(CB, Stats, Persons, A.P_Self, A, AdvGame, true, 1 + CB.MCE_Choice_Off);
            List<int> cFollower;
            cFollower = new List<int>();

            // 1 
            cFollower.Add(1);
            mcM.Add(new MCMenuEntry(0, "Willst du das Spiel wirklich neu starten?", 1, 0, false));

            // 2 
            cFollower.Add(2);
            mcM.Add(new MCMenuEntry(A.P_Self, "Ja", 2, -1, false));
            mcM.Last().Del = AdvGame.DoMCRestart;

            // 3 
            cFollower.Add(3);
            mcM.Add(new MCMenuEntry(A.P_Self, "Nein", 3, -1, false));

            mcM.Add(new MCMenuEntry(1 + CB.MCE_Choice_Off, cFollower));

            mcM.MCS = mcM.MenuShow();
            mcM.Set(0);
            mcM.MCS.MCOutput(mcM, mcM.MCSelection, false);


            return (true);
        }

        public virtual bool ExamineIn(int PersonID, ParseTokenList PTL)
        {
            Item item1 = PTL.GetFirstItem(); //  GetItemRef(Adv_PT[2].WordID);
            AdvGame.StoryOutput(Persons.Find(PersonID).LocationID, A.P_Everyone, "<br><i>" + Persons.GetPersonVerbLink(PersonID, Co.CASE_AKK, CB.VT_schauen, A.Tense) + " in " + Items.GetItemNameLink(item1.ID, Co.CASE_NOM) + ". (" + item1.ID + ")</i>");
            ListItems(Persons.GetPersonVerbLink(PersonID, Co.CASE_AKK, CB.VT_sehen, A.Tense) + ": ", PersonID, CB.LocType_In_Item, item1.ID, true, false);
            return (true);

        }
        public virtual bool ExamineOn(int PersonID, ParseTokenList PTL)
        {
            Item item1 = PTL.GetFirstItem(); //  GetItemRef(Adv_PT[2].WordID);
            AdvGame.StoryOutput(Persons.Find(PersonID).LocationID, A.P_Everyone, "<br><i>" + Persons.GetPersonVerbLink(PersonID, Co.CASE_AKK, CB.VT_schauen, A.Tense) + " auf " + Items.GetItemNameLink(item1.ID, Co.CASE_NOM) + ". (" + item1.ID + ")</i>");
            ListItems(Persons.GetPersonVerbLink(PersonID, Co.CASE_AKK, CB.VT_sehen, A.Tense) + ": ", PersonID, CB.LocType_On_Item, item1.ID, true, false);
            return (true);

        }
        public virtual bool ExamineBehind(int PersonID, ParseTokenList PTL)
        {
            Item item1 = PTL.GetFirstItem(); //  GetItemRef(Adv_PT[2].WordID);
            AdvGame.StoryOutput(Persons.Find(PersonID).LocationID, A.P_Everyone, "<br><i>" + Persons.GetPersonVerbLink(PersonID, Co.CASE_AKK, CB.VT_schauen, A.Tense) + " hinter " + Items.GetItemNameLink(item1.ID, Co.CASE_NOM) + ". (" + item1.ID + ")</i>");
            ListItems(Persons.GetPersonVerbLink(PersonID, Co.CASE_AKK, CB.VT_sehen, A.Tense) + ": ", PersonID, CB.LocType_Behind_Item, item1.ID, true, false);
            return (true);
        }
        public virtual bool ExamineBeside(int PersonID, ParseTokenList PTL)
        {
            Item item1 = PTL.GetFirstItem(); //  GetItemRef(Adv_PT[2].WordID);
            AdvGame.StoryOutput(Persons.Find(PersonID).LocationID, A.P_Everyone, "<br><i>" + Persons.GetPersonVerbLink(PersonID, Co.CASE_AKK, CB.VT_schauen, A.Tense) + " neben " + Items.GetItemNameLink(item1.ID, Co.CASE_NOM) + ". (" + item1.ID + ")</i>");
            ListItems(Persons.GetPersonVerbLink(PersonID, Co.CASE_AKK, CB.VT_sehen, A.Tense) + ": ", PersonID, CB.LocType_Beside_Item, item1.ID, true, false);
            return (true);
        }

        public virtual bool SaveMC(int PersoniD, ParseTokenList PTL)
        {
            int idCt = 1;
            // char Key = '1';

            MCMenu mcM = AdvGame.AdvMCMenu(1, false, 1 + CB.MCE_Choice_Off);
            List<int> follower;


            follower = new List<int>();
            follower.Add(-1);
            mcM.Add(new MCMenuEntry(CB.MCE_Text, 0, "Speichern in welchem Slot?", idCt++, follower, 0, 0, false, false, false, null, null));

            for (int i = 0; i < 10; i++)
            {
                int Val = i + 1;

                mcM.Add(new MCMenuEntry(CB.MCE_Text, 1, "Slot " + $"{Val:00}", idCt++, follower, 0, 0, false, false, false, null, "speicher " + $"{Val:00}"));

            }


            mcM.Add(new MCMenuEntry(CB.MCE_Text, 1, "Abbruch", idCt++, follower, 0, 0, false, false, false, null, null));

            follower = new List<int>();
            for (int j = 1; j < idCt; j++)
            {
                follower.Add(j);

            }
            mcM.Add(new MCMenuEntry(CB.MCE_Choice, 0, "", 1 + CB.MCE_Choice_Off, follower, 0, 0, false, false, false, null, null));

            mcM.MCS = mcM.MenuShow();
            mcM.Set(0);
            // MCM.AddCurrent(1);
            mcM.MCS.MCOutput(mcM, SaveSelection, false);
            return (true);
        }
        public bool SaveSelection(MCMenu MCM, int Selection)
        {
            if (Selection == -1)
            {
               MCM.OutputExit();
            }
            else if (Selection == -2)
            {
                MCM.OutputExit();
            }
            else 
            {
                ParseTokenList PT = new ParseTokenList(Verbs, Preps, Nouns, Adjs, Fills);

                PT.AddNothing(0);
                PT.AddPrep(Selection - 1 + CB.PrepID_Slot1 - 1);

                Save(A.ActPerson, PT);
                MCM.OutputExit();
            }

            return (true);
        }
        public bool LoadSelection(MCMenu MCM, int Selection)
        {
            if (Selection == -1)
            {
                MCM.OutputExit();
            }
            else if (Selection == -2)
            {
                MCM.OutputExit();
            }
            else
            {
                ParseTokenList PT = new ParseTokenList(Verbs, Preps, Nouns, Adjs, Fills);

                if (MCM.FindID(Selection).Text.Substring(0, 5) == "Slot ")
                {
                    int SlotID = Convert.ToInt32(MCM.FindID(Selection).Text.Substring(5, 2)) + CB.PrepID_Slot1 - 1 ;

                    PT.AddNothing(0);
                    PT.AddPrep(SlotID);

                    Load(A.ActPerson, PT);
                    AdvGame.DoUIUpdate();
                    MCM.OutputExit();
                }
                else
                {
                    MCM.OutputExit();
                }

            }

            return (true);
        }
        public virtual bool LoadMC(int PersoniD, ParseTokenList PTL)
        {
            int idCt = 1;
            // char Key = '1';

            MCMenu mcM = AdvGame.AdvMCMenu(1, false, 1 + CB.MCE_Choice_Off);
            List<int> follower;


            follower = new List<int>();
            follower.Add(-1);
            mcM.Add(new MCMenuEntry(CB.MCE_Text, 0, "Laden aus welchem Slot?", idCt++, follower, 0, 0, false, false, false, null, null));

            for (int i = 0; i < 10; i++)
            {
                int Val = i + 1;
                string UP = System.Environment.GetEnvironmentVariable("USERPROFILE");
                string pathName = UP + "\\documents\\My Games\\Advtest";
                string fileName = pathName + "\\slot" + Val + ".sav";

                if (File.Exists(fileName))
                {
                    mcM.Add(new MCMenuEntry(CB.MCE_Text, 1, "Slot " + $"{Val:00}", idCt++, follower, 0, 0, false, false, false, null, "lade " + $"{Val:00}"));
                    // IDCt++;
                }
            }


            mcM.Add(new MCMenuEntry(CB.MCE_Text, 1, "Abbruch", idCt++, follower, 0, 0, false, false, false, null, null));

            follower = new List<int>();
            for (int j = 1; j < idCt; j++)
            {
                follower.Add(j);

            }
            mcM.Add(new MCMenuEntry(CB.MCE_Choice, 0, "", 1 + CB.MCE_Choice_Off, follower, 0, 0, false, false, false, null, null));

            mcM.MCS = mcM.MenuShow(); ;
            mcM.Set(0);
            // MCM.AddCurrent(1);
            mcM.MCS.MCOutput(mcM, LoadSelection, false);
            return (true);

        }

        public void CoreSave( string fileName)
        {
            string up = System.Environment.GetEnvironmentVariable("USERPROFILE");
            string pathName = up + "\\documents\\My Games\\Advtest";
            string pathfileName = pathName + "\\"+fileName;

            if (!System.IO.Directory.Exists(pathName))
            {
                string p1 = up + "\\documents\\My Games\\Advtest";
                System.IO.Directory.CreateDirectory(p1);
                FileInfo filePath = new FileInfo(p1);
            }


            SaveObj SO = new SaveObj();

            SO.jsonItems = Items;
            SO.jsonLocations = Locations;
            SO.jsonPersons = Persons;
            SO.jsonTopics = Topics;
            SO.jsonAdjs = Adjs;
            SO.jsonA = A;
            SO.jsonNouns = Nouns;
            SO.jsonVerbs = Verbs;
            SO.jsonPreps = Preps;
            SO.jsonFills = Fills;
            SO.jsonStats = Stats;
            SO.jsonLI = LI;
            SO.jsonCA = CA;
            SO.jsonCB = CB;
            SO.jsonStoryText = AdvGame.STE;
            SO.jsonFeedbackText = AdvGame.FBE;
            SO.jsonFeedbackText.FeedbackModeMC = false;
            SO.jsonVerbTenses = AdvGame.VerbTenses;
                // SO.jsonParser = AdvGame.Parser;



            // SO.jsonMCE = MCE;
            // SO.jsonMCV = MCV;


            IFormatter formatter = new BinaryFormatter();

            Stream stream = new FileStream(pathfileName, FileMode.Create, FileAccess.Write, FileShare.None);
            formatter.Serialize(stream, SO);
            stream.Close();

        }
        public virtual bool Save(int PersonID, ParseTokenList PTL)
        {
            int slotID = ((Prep)PTL.Index(1).O).ID;
            int slotNr;

            if (slotID == CB.PrepID_Slot1) slotNr = 1;
            else if (slotID == CB.PrepID_Slot2) slotNr = 2;
            else if (slotID == CB.PrepID_Slot3) slotNr = 3;
            else if (slotID == CB.PrepID_Slot4) slotNr = 4;
            else if (slotID == CB.PrepID_Slot5) slotNr = 5;
            else if (slotID == CB.PrepID_Slot6) slotNr = 6;
            else if (slotID == CB.PrepID_Slot7) slotNr = 7;
            else if (slotID == CB.PrepID_Slot8) slotNr = 8;
            else if (slotID == CB.PrepID_Slot9) slotNr = 9;
            else if (slotID == CB.PrepID_Slot10) slotNr = 10;
            else slotNr = 1;

            CoreSave( "slot"+slotNr + ".sav");

            /*
            string up = System.Environment.GetEnvironmentVariable("USERPROFILE");
            string pathName = up + "\\documents\\My Games\\Advtest";
            string fileName = pathName + "\\slot" + slotNr + ".sav";

            if (!System.IO.Directory.Exists(pathName))
            {
                string p1 = up + "\\documents\\My Games\\Advtest";
                System.IO.Directory.CreateDirectory(p1);
                FileInfo filePath = new FileInfo(p1);
            }


            SaveObj SO = new SaveObj();

            SO.jsonItems = Items;
            SO.jsonLocations = Locations;
            SO.jsonPersons = Persons;
            SO.jsonTopics = Topics;
            SO.jsonAdjs = Adjs;
            SO.jsonA = A;
            SO.jsonNouns = Nouns;
            SO.jsonVerbs = Verbs;
            SO.jsonPreps = Preps;
            SO.jsonFills = Fills;
            SO.jsonStats = Stats;
            SO.jsonLI = LI;
            SO.jsonCA = CA;
            SO.jsonCB = CB;
            // SO.jsonParser = AdvGame.Parser;



            // SO.jsonMCE = MCE;
            // SO.jsonMCV = MCV;


            IFormatter formatter = new BinaryFormatter();

            Stream stream = new FileStream(fileName, FileMode.Create, FileAccess.Write, FileShare.None);
            formatter.Serialize(stream, SO);
            stream.Close();

            // jsonString = JsonSerializer.Serialize(SO);
            // File.WriteAllText(fileName, jsonString);
            */

            AdvGame.FeedbackOutput(PersonID, "Spielstand in Slot " + slotNr + " gespeichert.");
            return (true);
        }

        public string PathFileNameFromSlot( int SlotNr )
        {
            string up = System.Environment.GetEnvironmentVariable("USERPROFILE");
            string pathName = up + "\\documents\\My Games\\Advtest";
            string pathfileName = pathName + "\\slot" + SlotNr+".sav";

            return pathfileName;
        }
        public string PathFileNameFromFileName(string FileName)
        {
            string up = System.Environment.GetEnvironmentVariable("USERPROFILE");
            string pathName = up + "\\documents\\My Games\\Advtest";
            string pathfileName = pathName + "\\"+FileName;

            return pathfileName;
        }

        public void CoreLoad( string fileName )
        {
            string up = System.Environment.GetEnvironmentVariable("USERPROFILE");
            string pathName = up + "\\documents\\My Games\\Advtest";
            string pathfileName = pathName + "\\"+fileName;

            IFormatter formatter = new BinaryFormatter();
            Stream stream = new FileStream(pathfileName, FileMode.Open, FileAccess.Read, FileShare.Read);
            SaveObj SO = (SaveObj)formatter.Deserialize(stream);
            stream.Close();
                       
            AdvGame.Items = Items = SO.jsonItems;
            AdvGame.Persons = Persons = SO.jsonPersons;
            AdvGame.Locations = Locations = SO.jsonLocations;
            AdvGame.Adjs = Adjs = SO.jsonAdjs;
            AdvGame.A = A= SO.jsonA;
            AdvGame.Nouns = Nouns = SO.jsonNouns;
            AdvGame.Verbs = Verbs = SO.jsonVerbs;
            AdvGame.Preps = Preps = SO.jsonPreps;
            AdvGame.Fills = Fills = SO.jsonFills;
            AdvGame.Stats = Stats = SO.jsonStats;
            AdvGame.LI = LI = SO.jsonLI;
            AdvGame.Topics = Topics = SO.jsonTopics;
            AdvGame.CA = CA = Co.CA = SO.jsonCA;
            AdvGame.CB = CB = Co.CB = SO.jsonCB;
            AdvGame.STE = SO.jsonStoryText;
            AdvGame.FBE = SO.jsonFeedbackText;
            AdvGame.VerbTenses = SO.jsonVerbTenses;
            // AdvGame.Parser = SO.jsonParser;

            // Locations.SetMW( MW );
            AdvGame.SetCallbacks();
            Locations.SetAdv(AdvGame);
            Persons.SetAdv(AdvGame);
            Items.SetAdvGame(AdvGame);
            AdvGame.SetObjectReferences(Items, Persons, Locations, Nouns, Adjs, Verbs, Preps, Fills, Stats, LI, Topics, CA, CB);
            AdvGame.Parser.SetObjectReferences(Items, Persons, Nouns, Adjs, Verbs, Preps, Fills, Topics, AdvGame.PLL);
            AdvGame.InitPLL();
            AdvGame.ResetParser();
            AdvGame.ResetStoryText();
            AdvGame.ResetFeedbackText();
            AdvGame.A.Adventure = AdvGame;
            Grammar.Init(AdvGame.A, AdvGame.VerbTenses, Items, Persons);

            foreach (Person p in Persons.List)
            {
                if (p.LatestDialog != null)
                {
                    p.LatestDialog.AdvGame = AdvGame;
                    p.LatestDialog.MCS = null;
                }
            }

        }
        public virtual bool Load(int PersonID, ParseTokenList PTL)
        {
            int slotID = ((Prep)PTL.Index(1).O).ID;
            int slotNr;

            if (slotID == CB.PrepID_Slot1) slotNr = 1;
            else if (slotID == CB.PrepID_Slot2) slotNr = 2;
            else if (slotID == CB.PrepID_Slot3) slotNr = 3;
            else if (slotID == CB.PrepID_Slot4) slotNr = 4;
            else if (slotID == CB.PrepID_Slot5) slotNr = 5;
            else if (slotID == CB.PrepID_Slot6) slotNr = 6;
            else if (slotID == CB.PrepID_Slot7) slotNr = 7;
            else if (slotID == CB.PrepID_Slot8) slotNr = 8;
            else if (slotID == CB.PrepID_Slot9) slotNr = 9;
            else if (slotID == CB.PrepID_Slot10) slotNr = 10;
            else slotNr = 1;

            if (!File.Exists(PathFileNameFromSlot( slotNr )))
            {
                AdvGame.FeedbackOutput(PersonID, "Spielstand in Slot " + slotNr + " existiert nicht.");
                return (false);
            }

            CoreLoad("slot" + slotNr + ".sav");

            /*
            SaveObj SO = new SaveObj();

            SO.jsonIT = IT;
            SO.jsonLOC = Loc;
            SO.jsonADJ = ADJ;
            SO.jsonA = A;
            SO.jsonNN = NN;
            SO.jsonOrd = Ord;
            SO.jsonPR = PR;
            SO.jsonFL = FL;

            jsonString = File.ReadAllText(fileName);
            SO = JsonSerializer.Deserialize<SaveObj>(jsonString);
            */

            AdvGame.FeedbackOutput(PersonID, "Spielstand aus Slot " + slotNr + " geladen.");
            Locations.ShowLocationFull(A.ActLoc);
            return (true);
        }

        public virtual bool Help(ParseTokenList PTL)
        {
            return true;
        }

        // Hilfsfunktion der Order-Klasse
        public bool ListItems(string InitText, int PersonID, int LocType, int LocID, bool ShowHidden, bool SuppressComment)
        {
            int i = 0;

            foreach (Item tItem in Items.List)
            {
                if ((tItem.LocationType == LocType) && (tItem.LocationID == LocID) && (tItem.IsBackground == false) && ((tItem.IsHidden == false) || (ShowHidden == true)))
                    i++;
            }
            if (i > 0)
            {
                int Found = 0;

                AdvGame.StoryOutput(Persons.Find(PersonID).LocationID, A.P_Everyone, InitText);

                for (int j = 0; j < Items.List.Count; j++)
                {
                    if ((Items.List[j].LocationType == LocType) && (Items.List[j].LocationID == LocID) && (Items.List[j].IsBackground == false) && ((Items.List[j].IsHidden == false) || (ShowHidden == true)))
                    {
                        // MW.TextOutput("- <a href=\"https:www.spiegel.de\" onclick=\"var myMenu = new Menu(); myMenu.addMenuItem(\"my menu item A\"); myMenu.addMenuItem(\"my menu item B\"); myMenu.addMenuItem(\"my menu item C\"); myMenu.addMenuItem(\"my menu item D\"); myMenu.writeMenus();\">"+ Items.GetItemNameLink(IT[j].ID, Co.CASE_NOM_UNDEF)+"</a>");
                        AdvGame.StoryOutput(Persons.Find(PersonID).LocationID, A.P_Everyone, "- " + Items.GetItemNameLink(Items.List[j].ID, Co.CASE_NOM_UNDEF));
                        // Items.List[j].IsHidden = false;
                        Found++;
                        if (Items.List[j].IsHidden == true) Items.List[j].IsHidden = false;

                    }
                }
            }
            else if (!SuppressComment)
                AdvGame.StoryOutput(Persons.Find(PersonID).LocationID, A.P_Everyone, "Nichts zu sehen.");
            return (true);
        }
        public virtual bool ProcessTake(Item I, int PersonID)
        {
            if (Items.IsItemHere(I, Co.Range_Here))
            {
                if (I.CanBeTaken == false)
                    AdvGame.FeedbackOutput(PersonID, Persons.GetPersonVerbLink(PersonID, Co.CASE_AKK, CB.VT_koennen, A.Tense) + " " + Items.GetItemNameLink(I.ID, Co.CASE_NOM) + " nicht nehmen.");
                else if ((I.LocationType == CB.LocType_Person) && (I.LocationID == PersonID))
                    AdvGame.StoryOutput(Persons.Find(PersonID).LocationID, PersonID, "<br><i>Du hast " + Items.GetItemNameLink(I.ID, Co.CASE_NOM) + " bereits</i>");
                else
                {
                    if (I.LocationType == CB.LocType_In_Item)
                    {
                        Item item2 = Items.Find(I.LocationID);
                        AdvGame.StoryOutput(Persons.Find(PersonID).LocationID, A.P_Everyone, "<br><i>" + Persons.GetPersonVerbLink(PersonID, Co.CASE_AKK, CB.VT_nehmen, A.Tense) + " " + Items.GetItemNameLink(I.ID, Co.CASE_NOM) + " aus " + Items.GetItemNameLink(item2.ID, Co.CASE_DAT) + ".(" + I.ID + ")</i>");
                    }
                    else if (I.LocationType == CB.LocType_On_Item)
                    {
                        Item item2 = Items.Find(I.LocationID);
                        AdvGame.StoryOutput(Persons.Find(PersonID).LocationID, A.P_Everyone, "<br><i>" + Persons.GetPersonVerbLink(PersonID, Co.CASE_AKK, CB.VT_nehmen, A.Tense) + " " + Items.GetItemNameLink(I.ID, Co.CASE_NOM) + " von " + Items.GetItemNameLink(item2.ID, Co.CASE_DAT) + ".(" + I.ID + ")</i>");
                    }
                    else if (I.LocationType == CB.LocType_Behind_Item)
                    {
                        Item item2 = Items.Find(I.LocationID);
                        AdvGame.StoryOutput(Persons.Find(PersonID).LocationID, A.P_Everyone, "<br><i>" + Persons.GetPersonVerbLink(PersonID, Co.CASE_AKK, CB.VT_holen, A.Tense) + " " + Items.GetItemNameLink(I.ID, Co.CASE_NOM) + " hinter " + Items.GetItemNameLink(item2.ID, Co.CASE_DAT) + " hervor. (" + I.ID + ")</i>");
                    }
                    else if (I.LocationType == CB.LocType_Below_Item)
                    {
                        Item item2 = Items.Find(I.LocationID);
                        AdvGame.StoryOutput(Persons.Find(PersonID).LocationID, A.P_Everyone, "<br><i>" + Persons.GetPersonVerbLink(PersonID, Co.CASE_AKK, CB.VT_ziehen, A.Tense) + " " + Items.GetItemNameLink(I.ID, Co.CASE_NOM) + " unter " + Items.GetItemNameLink(item2.ID, Co.CASE_DAT) + " hervor. (" + I.ID + ")</i>");
                    }
                    else if (I.LocationType == CB.LocType_Beside_Item)
                    {
                        Item item2 = Items.Find(I.LocationID);
                        AdvGame.StoryOutput(Persons.Find(PersonID).LocationID, A.P_Everyone, "<br><i>" + Persons.GetPersonVerbLink(PersonID, Co.CASE_AKK, CB.VT_nehmen, A.Tense) + " " + Items.GetItemNameLink(I.ID, Co.CASE_NOM) + " neben " + Items.GetItemNameLink(item2.ID, Co.CASE_DAT) + " weg.(" + I.ID + ")</i>");
                    }
                    else
                    {
                        AdvGame.StoryOutput(Persons.Find(PersonID).LocationID, A.P_Everyone, "<br><i>" + Persons.GetPersonVerbLink(PersonID, Co.CASE_AKK, CB.VT_nehmen, A.Tense) + " " + Items.GetItemNameLink(I.ID, Co.CASE_NOM) + ". (" + I.ID + ")</i>");
                    }
                    Items.TransferItem(I, CB.LocType_Person, PersonID);
                    I.IsBackground = false;
                    I.IsHidden = false;
                    // I.IsHidden = false;
                }
            }
            else
            {
                AdvGame.StoryOutput(Persons.Find(PersonID).LocationID, A.P_Everyone, "<br><i>" + Persons.GetPersonVerbLink(PersonID, Co.CASE_AKK, CB.VT_schauen, A.Tense) + " " + Grammar.GetReflexivePronoun(PersonID, Co.CASE_AKK) + " um, aber " + Persons.GetPersonVerbLink(PersonID, Co.CASE_AKK, CB.VT_finden, A.Tense) + " nichts.</i>");
            }
            return (true);

        }
        public virtual bool ProcessTakeP(Person P, int PersonID )
        {
            if (Persons.IsPersonHere(P, Co.Range_Here))
            {
                if (P.CanBeTaken == false)
                    AdvGame.FeedbackOutput(PersonID, Persons.GetPersonVerbLink(PersonID, Co.CASE_AKK, CB.VT_koennen, A.Tense) + " " + Persons.GetPersonNameLink(P.ID, Co.CASE_NOM) + " nicht nehmen.");
                else if ((P.LocationType == CB.LocType_Person) && (P.LocationID == PersonID))
                    AdvGame.StoryOutput(Persons.Find(PersonID).LocationID, A.P_Everyone, "<br><i>" + Persons.GetPersonVerbLink(PersonID, Co.CASE_AKK, CB.VT_haben, A.Tense) + " " + Persons.GetPersonNameLink(P.ID, Co.CASE_NOM) + " bereits</i>");
                else
                {
                    if (P.LocationType == CB.LocType_In_Item)
                    {
                        Item item2 = Items.Find(P.LocationID);
                        AdvGame.StoryOutput(Persons.Find(PersonID).LocationID, A.P_Everyone, "<br><i>" + Persons.GetPersonVerbLink(PersonID, Co.CASE_AKK, CB.VT_nehmen, A.Tense) + " " + Persons.GetPersonNameLink(P.ID, Co.CASE_NOM) + " aus " + Items.GetItemNameLink(item2.ID, Co.CASE_DAT) + ".(" + P.ID + ")</i>");
                    }
                    else if (P.LocationType == CB.LocType_On_Item)
                    {
                        Item item2 = Items.Find(P.LocationID);
                        AdvGame.StoryOutput(Persons.Find(PersonID).LocationID, A.P_Everyone, "<br><i>" + Persons.GetPersonVerbLink(PersonID, Co.CASE_AKK, CB.VT_nehmen, A.Tense) + " " + Persons.GetPersonNameLink(P.ID, Co.CASE_NOM) + " von " + Items.GetItemNameLink(item2.ID, Co.CASE_DAT) + ".(" + P.ID + ")</i>");
                    }
                    else if (P.LocationType == CB.LocType_Behind_Item)
                    {
                        Item item2 = Items.Find(P.LocationID);
                        AdvGame.StoryOutput(Persons.Find(PersonID).LocationID, A.P_Everyone, "<br><i>" + Persons.GetPersonVerbLink(PersonID, Co.CASE_AKK, CB.VT_holen, A.Tense) + " " + Persons.GetPersonNameLink(P.ID, Co.CASE_NOM) + " hinter " + Items.GetItemNameLink(item2.ID, Co.CASE_DAT) + " hervor. (" + P.ID + ")</i>");
                    }
                    else if (P.LocationType == CB.LocType_Below_Item)
                    {
                        Item item2 = Items.Find(P.LocationID);
                        AdvGame.StoryOutput(Persons.Find(PersonID).LocationID, A.P_Everyone, "<br><i>" + Persons.GetPersonVerbLink(PersonID, Co.CASE_AKK, CB.VT_ziehen, A.Tense) + " " + Persons.GetPersonNameLink(P.ID, Co.CASE_NOM) + " unter " + Items.GetItemNameLink(item2.ID, Co.CASE_DAT) + " hervor. (" + P.ID + ")</i>");
                    }
                    else if (P.LocationType == CB.LocType_Beside_Item)
                    {
                        Item item2 = Items.Find(P.LocationID);
                        AdvGame.StoryOutput(Persons.Find(PersonID).LocationID, A.P_Everyone, "<br><i>" + Persons.GetPersonVerbLink(PersonID, Co.CASE_AKK, CB.VT_nehmen, A.Tense) + " " + Persons.GetPersonNameLink(P.ID, Co.CASE_NOM) + " neben " + Items.GetItemNameLink(item2.ID, Co.CASE_DAT) + " weg.(" + P.ID + ")</i>");
                    }
                    else
                    {
                        AdvGame.StoryOutput(Persons.Find(PersonID).LocationID, A.P_Everyone, "<br><i>" + Persons.GetPersonVerbLink(PersonID, Co.CASE_AKK, CB.VT_nehmen, A.Tense) + " " + Persons.GetPersonNameLink(P.ID, Co.CASE_NOM) + ". (" + P.ID + ")</i>");
                    }
                    Persons.TransferPerson(P, CB.LocType_Person, PersonID);
                }
            }
            else
            {
                AdvGame.StoryOutput(Persons.Find(PersonID).LocationID, A.P_Everyone, "<br><i>" + Persons.GetPersonVerbLink(PersonID, Co.CASE_AKK, CB.VT_schauen, A.Tense) + " " + Grammar.GetReflexivePronoun(PersonID, Co.CASE_AKK) + " um, aber findest nichts.</i>");
            }
            return (true);

        }

        public virtual bool MCItem(int PersonID, ParseTokenList PTL)
        {
            Item item1 = PTL.GetFirstItem();
            return (AdvGame.DoMCItem(item1.ID));
        }
        public virtual bool MCPerson(int PersonID, ParseTokenList PTL)
        {
            Person person1 = PTL.GetFirstPerson();
            return (AdvGame.DoMCPerson(person1.ID));
        }
    }
}