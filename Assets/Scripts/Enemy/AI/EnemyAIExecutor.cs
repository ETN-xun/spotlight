using System.Collections;
using System.Collections.Generic;
using Action;
using UnityEngine;

namespace Enemy.AI
{
    public class EnemyAIExecutor : MonoBehaviour
    {
        public static EnemyAIExecutor Instance { get; private set; }
        [SerializeField] private float actionDelay = 0.25f;

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }
            Instance = this;
        }

        public static EnemyAIExecutor EnsureInstance()
        {
            if (Instance == null)
            {
                var go = new GameObject("EnemyAIExecutor");
                Instance = go.AddComponent<EnemyAIExecutor>();
            }
            return Instance;
        }

        public void RunEnemyTurn()
        {
            StartCoroutine(Run());
        }

        private IEnumerator Run()
        {
            // 收集所有敌方单位
            var enemies = GameObject.FindObjectsOfType<Unit>();
            var list = new List<Unit>();
            foreach (var u in enemies)
            {
                if (u != null && u.data != null && u.data.isEnemy)
                {
                    list.Add(u);
                }
            }

            foreach (var enemy in list)
            {
                if (enemy == null || enemy.CurrentCell == null) continue;
                var intent = EnemyIntentPlanner.BuildForUnit(enemy);
                if (intent == null) continue;
                var plan = intent.selectedPlan;
                if (plan == null || plan.Count == 0)
                {
                    // fallback: 兼容旧逻辑
                    if (intent.selected != null)
                    {
                        yield return ExecuteIntent(enemy, intent.selected);
                        yield return new WaitForSeconds(actionDelay);
                    }
                    continue;
                }
                foreach (var step in plan)
                {
                    yield return ExecuteIntent(enemy, step);
                    yield return new WaitForSeconds(actionDelay);
                }
            }

            // 敌人回合结束
            GameManager.Instance.EndCurrentTurn();
        }

        private IEnumerator ExecuteIntent(Unit enemy, EnemyIntent.IntentData sel)
        {
            switch (sel.action)
            {
                case ActionType.Move:
                    yield return ExecuteMove(enemy, sel);
                    break;
                case ActionType.Attack:
                    yield return ExecuteBasicAttack(enemy, sel);
                    break;
                case ActionType.Ability:
                    yield return ExecuteAbility(enemy, sel);
                    break;
            }
        }

        private IEnumerator ExecuteMove(Unit enemy, EnemyIntent.IntentData sel)
        {
            var dest = sel.target.hasCell ? sel.target.cell : (sel.path.cells != null && sel.path.cells.Count > 0 ? sel.path.cells[sel.path.cells.Count - 1] : enemy.CurrentCell.Coordinate);
            GridManager.Instance.MoveUnit(dest, enemy);
            yield return null;
        }

        private IEnumerator ExecuteBasicAttack(Unit enemy, EnemyIntent.IntentData sel)
        {
            if (!sel.target.hasCell) yield break;
            var cell = GridManager.Instance.GetCell(sel.target.cell);
            if (cell == null) yield break;
            var target = cell.CurrentUnit;
            if (target == null) yield break;
            // 使用单位基础伤害
            target.TakeDamage(enemy.data.baseDamage);
            yield return null;
        }

        private IEnumerator ExecuteAbility(Unit enemy, EnemyIntent.IntentData sel)
        {
            // 根据 abilityId 找到技能数据
            SkillDataSO skillData = null;
            foreach (var s in enemy.data.skills)
            {
                if (s != null && s.skillID == sel.abilityId)
                {
                    skillData = s;
                    break;
                }
            }
            if (skillData == null) yield break;

            // 选择目标格（中心），若没有记录则用敌人当前位置
            Vector2Int centerCell = sel.target.hasCell ? sel.target.cell : enemy.CurrentCell.Coordinate;
            var targetCell = GridManager.Instance.GetCell(centerCell);
            if (targetCell == null) yield break;

            var skill = CreateSkill(skillData, enemy);
            if (skill == null) yield break;

            skill.Execute(targetCell, GridManager.Instance);
            yield return null;
        }

        private Skill CreateSkill(SkillDataSO data, Unit caster)
        {
            switch (data.skillType)
            {
                case SkillType.Damage:
                    return new DamageSkill(data, caster);
                case SkillType.Displacement:
                    return new DisplacementSkill(data, caster);
                case SkillType.Spawn:
                    return new SpawnSkill(data, caster);
                default:
                    return null;
            }
        }
    }
}
