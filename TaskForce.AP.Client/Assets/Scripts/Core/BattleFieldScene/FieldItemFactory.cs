using System;

namespace TaskForce.AP.Client.Core.BattleFieldScene
{
    public class FieldItemFactory
    {
        private readonly Func<string, View.BattleFieldScene.IFieldItem> _createView;
        private readonly GameDataStore _gameDataStore;

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

            switch (fieldItemID)
            {
                case GameData.FieldItemID.MedicalKit:
                    return new MedicalKit(_createView(data.BodyID));
                default:
                    return null;
            }
        }
    }
}
