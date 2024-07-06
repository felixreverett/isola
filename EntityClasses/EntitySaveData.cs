namespace FeloxGame.EntityClasses
{
    public class EntitySaveData
    {
        public eEntityType EntityType { get; set; }
        public List<object> Data { get; set; }

        public EntitySaveData(eEntityType EntityType, List<object> data)
        {
            this.EntityType = EntityType;
            this.Data = data;
        }
    }
}
