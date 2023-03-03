using System.Collections.Generic;
using Verse;

namespace HRF
{
    public class SavedSkillRecordCollection : IExposable
    {
        public List<SavedSkillRecord> savedSkillRecords = new List<SavedSkillRecord>();
        public SavedSkillRecordCollection()
        {
            savedSkillRecords = new List<SavedSkillRecord>();
        }
        public void ExposeData()
        {
            Scribe_Collections.Look(ref savedSkillRecords, "savedSkillRecords", LookMode.Deep);
            if (savedSkillRecords is null)
            {
                savedSkillRecords = new List<SavedSkillRecord>();
            }
        }
    }
}
