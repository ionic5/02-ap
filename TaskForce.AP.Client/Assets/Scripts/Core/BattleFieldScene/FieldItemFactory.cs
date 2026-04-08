using System;

namespace TaskForce.AP.Client.Core.BattleFieldScene
{
    public class FieldItemFactory
    {
        private readonly Func<string, View.BattleFieldScene.IFieldItem> _createView;
        private readonly GameDataStore _gameDataStore;
        private readonly IExpOrbFinder _expOrbFinder;

        public event EventHandler<CreatedEventArgs<IFieldItem>> FieldItemCreatedEvent;

        public FieldItemFactory(Func<string, View.BattleFieldScene.IFieldItem> createView, GameDataStore gameDataStore, IExpOrbFinder expOrbFinder)
        {
            _createView = createView;
            _gameDataStore = gameDataStore;
            _expOrbFinder = expOrbFinder;
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
                case GameData.FieldItemID.GoldBundle:
                    fieldItem = new GoldBundle(_createView(data.BodyID));
                    break;
                case GameData.FieldItemID.Nuke:
                    fieldItem = new Nuke(_createView(data.BodyID));
                    break;
                case GameData.FieldItemID.Magnet:
                    fieldItem = new Magnet(_createView(data.BodyID), _expOrbFinder);
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
