using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Verse;

namespace TabulaRasa
{
    public class Dialog_NameThing : Window
    {
        private string curLabel;
        private Thing thing;

        public Dialog_NameThing(Thing thing)
        {
            this.forcePause = true;
            this.doCloseX = true;
            this.closeOnClickedOutside = true;
            this.absorbInputAroundWindow = true;
            this.closeOnClickedOutside = true;
            this.curLabel = thing.Label;
            this.thing = thing;
        }

        protected virtual int MaxNameLength
        {
            get
            {
                return 28;
            }
        }

        public override Vector2 InitialSize
        {
            get
            {
                return new Vector2(280f, 175f);
            }
        }

        protected virtual AcceptanceReport NameIsValid(string name)
        {
            if (name.Length == 0)
            {
                return false;
            }
            return true;
        }

        public override void DoWindowContents(Rect inRect)
        {
            Text.Font = GameFont.Medium;
            bool flag = false;
            if (Event.current.type == EventType.KeyDown && Event.current.keyCode == KeyCode.Return)
            {
                flag = true;
                Event.current.Use();
            }
            //GUI.SetNextControlName("RenameField");
            string text = this.curLabel;
            Widgets.Label(new Rect(15f, 15f, 500f, 50f), text);

            Text.Font = GameFont.Small;
            string text2 = Widgets.TextField(new Rect(15f, 50f, inRect.width - 15f - 15f, 35f), this.curLabel);

            if (text2.Length < this.MaxNameLength)
            {
                this.curLabel = text2;
            }
            if (Widgets.ButtonText(new Rect(15f, inRect.height - 35f - 15f, inRect.width - 15f - 15f, 35f), "OK", true, false, true) || flag)
            {
                AcceptanceReport acceptanceReport = this.NameIsValid(this.curLabel);
                if (!acceptanceReport.Accepted)
                {
                    if (acceptanceReport.Reason.NullOrEmpty())
                    {
                        Messages.Message("NameIsInvalid".Translate(), MessageTypeDefOf.RejectInput, false);
                    }
                    else
                    {
                        Messages.Message(acceptanceReport.Reason, MessageTypeDefOf.RejectInput, false);
                    }
                }
                else
                {
                    if (string.IsNullOrEmpty(curLabel))
                    {
                        curLabel = thing.Label.ToString();
                    }
                    thing.TryGetComp<Comp_Renameable>().customLabel = curLabel;
                    Find.WindowStack.TryRemove(this, true);
                    string msg = "Successfully renamed " + thing.def.label + " to '" + curLabel + "'";
                    Messages.Message(msg, this.thing, MessageTypeDefOf.PositiveEvent, false);
                }
            }
        }
    }
}
