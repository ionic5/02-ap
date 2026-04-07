using System;

namespace TaskForce.AP.Client.Core.BattleFieldScene
{
    public class FieldItemFactory
    {
        private readonly Func<View.BattleFieldScene.IFieldItem> _createView;

        public FieldItemFactory(Func<View.BattleFieldScene.IFieldItem> createView)
        {
            _createView = createView;
        }

        public MedicalKit CreateMedicalKit()
        {
            var view = _createView();
            return new MedicalKit(view);
        }
    }
}
