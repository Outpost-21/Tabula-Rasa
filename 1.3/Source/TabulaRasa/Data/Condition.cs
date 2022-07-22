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
    public struct Condition : IEquatable<Condition>
    {
        public ConditionType condition;
        public object data;

        public Condition(ConditionType condition, object data)
        {
            this.condition = condition;
            this.data = data;
        }

        public override string ToString()
        {
            return $"Condition_{condition}_{data}";
        }

        public bool Equals(Condition other)
        {
            return data == other.data && condition == other.condition;
        }

        public bool Passes(object toCheck)
        {
            switch (condition)
            {
                case ConditionType.IsType:
                    if (toCheck.GetType().ToString() == "Psychology.PsychologyPawn" && data.ToString() == "Verse.Pawn")
                    {
                        return true;
                    }
                    if (toCheck.GetType() == data.GetType() || Equals(toCheck.GetType(), data))
                    {
                        return true;
                    }
                    break;
                case ConditionType.IsTypeStringMatch:
                    if (toCheck.GetType().ToString() == (string)toCheck)
                    {
                        return true;
                    }
                    break;
                case ConditionType.ThingHasComp:
                    var dataTypeString = data?.ToString();
                    if (toCheck is ThingWithComps t && t.AllComps.Any(comp => comp?.props?.compClass is Type compClass && (compClass.ToString() == dataTypeString || compClass.BaseType?.ToString() == dataTypeString)))
                    {
                        return true;
                    }
                    break;
            }
            return false;
        }
    }
}
