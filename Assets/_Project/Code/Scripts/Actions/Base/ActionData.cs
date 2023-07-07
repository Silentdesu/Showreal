namespace TechnoDemo.Actions
{
    [System.Serializable]
    public struct ActionData
    {
        public int Id;
        public ActionTagSO Tag;
        public bool AutoStart;
        public ActionTagSO[] BlockedBy;
    }
}