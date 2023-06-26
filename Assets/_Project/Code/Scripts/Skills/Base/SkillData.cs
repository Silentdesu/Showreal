namespace TechnoDemo.Skills
{
    [System.Serializable]
    public struct SkillData
    {
        public SkillTagSO Tag;
        public bool AutoStart;
        public SkillTagSO[] BlockedBy;
    }
}