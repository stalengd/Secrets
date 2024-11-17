using UnityEngine;
using UnityEngine.Tilemaps;

namespace Anomalus.Rendering
{
    [CreateAssetMenu(menuName = "2D/Tiles/March Any Rule Tile")]
    public sealed class MatchAnyRuleTile : RuleTile
    {
        public override bool RuleMatch(int neighbor, TileBase tile)
        {
            return neighbor switch
            {
                TilingRuleOutput.Neighbor.NotThis => tile == null,
                TilingRuleOutput.Neighbor.This => tile != null,
                _ => base.RuleMatch(neighbor, tile),
            };
        }
    }
}
