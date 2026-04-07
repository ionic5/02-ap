using System;

namespace TaskForce.AP.Client.Core.BattleFieldScene
{
    public class FieldItemFactory
    {
        private readonly Func<string, View.BattleFieldScene.IFieldItem> _createView;
        private readonly GameDataStore _gameDataStore;

        public event EventHandler<CreatedEventArgs<IFieldItem>> FieldItemCreatedEvent;

        public FieldItemFactory(Func<string, View.BattleFieldScene.IFieldItem> createView, GameDataStore gameDataStore)
        {
            _createView = createView;
            _gameDataStore = gameDataStore;
        }

        public IFieldItem Create(string fieldItemID)
        {
            var data = _gameDataStore.GetFieldItem(fieldItemID);
            if (data == null)
                return null;

            IFieldItem fieldItem;
            switch (fieldItemID)
            {
                case GameData.FieldItemID.MedicalKit:
                    fieldItem = new MedicalKit(_createView(data.BodyID));
                    break;
                default:
                    return null;
            }

            EventHandler handler = null;
            handler = (sender, e) =>
            {
                fieldItem.SpawnCompletedEvent -= handler;
                FieldItemCreatedEvent?.Invoke(this, new CreatedEventArgs<IFieldItem>(fieldItem));
            };
            fieldItem.SpawnCompletedEvent += handler;

            return fieldItem;
        }
    }
}
