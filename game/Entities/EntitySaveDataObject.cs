namespace Isola.Entities
{
    public class EntitySaveDataObject
    {
        public eEntityType EntityType { get; set; }
        public string SaveDataString { get; set; }

        public EntitySaveDataObject(eEntityType entityType, string saveDataString)
        {
            this.EntityType = entityType;
            this.SaveDataString = saveDataString;
        }
    }
}
