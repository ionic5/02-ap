using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TaskForce.AP.Client.Core;
using TaskForce.AP.Client.Core.GameData;
using TaskForce.AP.Client.UnityWorld.AssetData;

namespace TaskForce.AP.Client.UnityWorld
{
    public class GameDataLoader
    {
        private readonly CsvLoader _csvLoader;

        public GameDataLoader(CsvLoader csvLoader)
        {
            _csvLoader = csvLoader;
        }

        public async Task Load(GameDataStore gameDataStore)
        {
            var loadTasks = new List<Task>
            {
                LoadConstants(gameDataStore),
                LoadTable(AssetID.ModifyAttributeEffect, row => new Core.GameData.ModifyAttributeEffect {
                    ID = row["id"],
                    ApplyOrder = int.Parse(row["applyOrder"]),
                    AttributeID = row["attributeID"],
                    CalculationType = row["calculationType"],
                    LevelCoefficientID = row["levelCoefficientID"]
                }, gameDataStore.AddModifyAttributeEffect),
                LoadTable(AssetID.LevelCoefficient, row => new Core.GameData.LevelCoefficient {
                    ID = row["id"],
                    Level = int.Parse(row["level"]),
                    Key = row["key"],
                    Value = float.Parse(row["value"])
                }, gameDataStore.AddLevelCoefficient),
                LoadTable(AssetID.StageEnemyUnit, row => new StageEnemyUnit {
                    StageLevel = int.Parse(row["stageLevel"]),
                    UnitID = row["unitID"],
                    Level = int.Parse(row["level"]),
                    SpawnRate = float.Parse(row["spawnRate"])
                }, gameDataStore.AddStageEnemyUnit),
                LoadTable(AssetID.Stage, row => new Stage {
                    Level = int.Parse(row["level"]),
                    Time = float.Parse(row["time"]),
                    SpawnGap = float.Parse(row["spawnGap"])
                }, gameDataStore.AddStage),
                LoadTable(AssetID.Unit, row => new Core.GameData.Unit {
                    ID = row["id"],
                    BodyID = row["bodyID"],
                    LevelAttributeID = row["levelAttributeID"]
                }, gameDataStore.AddUnit),
                LoadTable(AssetID.NonPlayerUnitLogic, row => new NonPlayerUnitLogic {
                    UnitID = row["unitID"],
                    UnitLogicID = row["unitLogicID"]
                }, gameDataStore.AddNonPlayerUnitLogic),
                LoadTable(AssetID.UnitDefaultSkill, row => new UnitDefaultSkill {
                    UnitID = row["unitID"],
                    SkillID = row["skillID"]
                }, gameDataStore.AddUnitDefaultSkill),
                LoadTable(AssetID.Skill, row => new Core.GameData.Skill {
                    ID = row["id"],
                    IconID = row["iconID"],
                    NameTextID = row["nameTextID"],
                    BaseAttributeID = row["baseAttributeID"],
                    LevelAttributeID = row["levelAttributeID"]
                }, gameDataStore.AddSkill),
                LoadTable(AssetID.LevelUpRewardSkill, row => new LevelUpRewardSkill {
                    SkillID = row["skillID"]
                }, gameDataStore.AddLevelUpRewardSkill),
                LoadTable(AssetID.LevelAttribute, row => new Core.GameData.LevelAttribute {
                    ID = row["id"],
                    Level = int.Parse(row["level"]),
                    AttributeID = row["attributeID"],
                    Value = CreateAttribute(row["value"])
                }, gameDataStore.AddLevelAttribute),
                LoadTable(AssetID.RequireExp, row => new Core.GameData.RequireExp {
                    Level = int.Parse(row["level"]),
                    Exp = int.Parse(row["exp"]),
                }, gameDataStore.AddRequireExp),
                LoadTable(AssetID.SoulExp, row => new Core.GameData.SoulExp {
                    Level = int.Parse(row["level"]),
                    Exp = int.Parse(row["exp"]),
                }, gameDataStore.AddSoulExp),
                LoadTable(AssetID.BaseAttribute, row => new Core.GameData.BaseAttribute {
                    ID = row["id"],
                    AttributeID = row["attributeID"],
                    Value = CreateAttribute(row["value"])
                }, gameDataStore.AddBaseAttribute),
                LoadTable(AssetID.ModifyAttributeSkill, row => new Core.GameData.ModifyAttributeSkill {
                    SkillID = row["skillID"],
                    ModifyAttributeEffectID = row["modifyAttributeEffectID"]
                }, gameDataStore.AddModifyAttributeSkill),
                LoadTable(AssetID.UnitDefaultActiveSkill, row => new Core.GameData.UnitDefaultActiveSkill {
                    UnitID = row["unitID"],
                    SkillID = row["skillID"]
                }, gameDataStore.AddUnitDefaultActiveSkill),
                LoadTable(AssetID.SkillDescription, (row) => {
                    var parameters = new List<object>();
                    for(var i= 0;i<2;i++) {
                        var param = row[$"param{i}"];
                        if(!string.IsNullOrEmpty(param))
                            parameters.Add(param);
                    }

                    return new Core.GameData.SkillDescription {
                        SkillID = row["skillID"],
                        Level = int.Parse(row["level"]),
                        TextID = row["textID"],
                        Parameters = parameters.ToArray()
                    };
                }, gameDataStore.AddSkillDescription)
            };

            await Task.WhenAll(loadTasks);

            gameDataStore.Bake();
        }

        private Core.Attribute CreateAttribute(string value)
        {
            if (float.TryParse(value, out var number))
                return new Core.Attribute(number);
            return new Core.Attribute(value);
        }

        private async Task LoadTable<T>(string assetId, Func<Dictionary<string, string>, T> parser, Action<T> adder)
        {
            var rows = await _csvLoader.LoadCsv(assetId);
            foreach (var row in rows)
            {
                adder(parser(row));
            }
        }

        private async Task LoadConstants(GameDataStore gameDataStore)
        {
            var rows = await _csvLoader.LoadCsv(AssetID.Constants);
            var map = rows.ToDictionary(row => row["constantID"], row => row["value"]);

            if (map.TryGetValue("SOUL_DROP_RATE", out var soulDropRate))
            {
                gameDataStore.SetSoulDropRate(float.Parse(soulDropRate));
            }

            if (map.TryGetValue("MAX_ENERGY", out var maxEnergy))
            {
                gameDataStore.SetMaxEnergy(int.Parse(maxEnergy));
            }

            if (map.TryGetValue("MINUTES_ENERGY_CHARGE", out var minutesEnergyCharge))
            {
                gameDataStore.SetMinutesEnergyCharge(int.Parse(minutesEnergyCharge));
            }

            if (map.TryGetValue("ENERGY_FOR_PLAY", out var energyForPlay))
            {
                gameDataStore.SetEnergyForPlay(int.Parse(energyForPlay));
            }

            if (map.TryGetValue("ENERGY_ADS_REWARD", out var energyAdsReward))
            {
                gameDataStore.SetEnergyAdsReward(int.Parse(energyAdsReward));
            }
        }
    }
}
