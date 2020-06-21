using System;
using System.Collections.Generic;
using System.Globalization;

namespace advtest
{

    [Serializable]
    public class Fill : Word
    {
        public Fill(int ID, string Name) : base(ID, Name)
        {
        }
    }
    [Serializable]
    public class FillList : WordList<Fill>
    {
        public FillList() : base()
        {
        }
        public override void Add(int ID, string name)
        {
            if (TList == null)
            {
                TList = new List<Fill>();
            }
            TList.Add(new Fill(ID, name));
        }
        public string GetFill(int FillID)
        {
            return (base.Find(FillID).Name);
        }
    }
}

