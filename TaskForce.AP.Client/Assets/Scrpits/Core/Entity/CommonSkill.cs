namespace TaskForce.AP.Client.Core.Entity
{
    public class CommonSkill : ActiveSkill, IActiveSkill
    {
        private readonly AttributeMediator _attributeMediator;

        public CommonSkill(string skillID, GameData.Skill skillData,
            TextStore textStore, AttributeMediator attributeMediator) : base(skillID, skillData, textStore)
        {
            _attributeMediator = attributeMediator;
        }

        public override void SetLevel(int value)
        {
            base.SetLevel(value);

            _attributeMediator.SetLevel(value);
        }

        public override Attribute GetAttribute(string attributeID)
        {
            return _attributeMediator.GetAttribute(attributeID);
        }
    }
}
