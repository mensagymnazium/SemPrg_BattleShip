using BattleShipEngine;

namespace BattleShipStrategies.Slavek.AI;

public partial record struct Experiences
{
    public static Experiences SmallDefault()
    {
        GameSetting s = new GameSetting(5,5, new int[] {2,1,1});
        Experiences e = new Experiences("SmallDefault", s,
            new CoefficientMap(s, new double[,] 
{{1,0,1,0,0},
{0,0,0,0,0},
{1,1,0,0,0},
{0,0,0,0,0},
{1,1,1,0,0}}),
            new Dictionary<(Int2,SlavekTile),CoefficientMap?>());
        e.AddChange(new Int2(0,0),SlavekTile.Boat, new CoefficientMap(s, new double[,] 
{{1,0,1,0,0},
{0,0,0,0,0},
{1,1,0,0,0},
{0,0,0,0,0},
{1,1,1,0,0}}));
        e.AddChange(new Int2(0,0),SlavekTile.Water, null);
        e.AddChange(new Int2(0,1),SlavekTile.Boat, null);
        e.AddChange(new Int2(0,1),SlavekTile.Water, new CoefficientMap(s, new double[,] 
{{1,0,1,0,0},
{0,0,0,0,0},
{1,1,0,0,0},
{0,0,0,0,0},
{1,1,1,0,0}}));
        e.AddChange(new Int2(0,2),SlavekTile.Boat, new CoefficientMap(s, new double[,] 
{{1,0,1,0,0},
{0,0,0,0,0},
{1,1,0,0,0},
{0,0,0,0,0},
{1,1,1,0,0}}));
        e.AddChange(new Int2(0,2),SlavekTile.Water, null);
        e.AddChange(new Int2(0,3),SlavekTile.Boat, null);
        e.AddChange(new Int2(0,3),SlavekTile.Water, new CoefficientMap(s, new double[,] 
{{1,0,1,0,0},
{0,0,0,0,0},
{1,1,0,0,0},
{0,0,0,0,0},
{1,1,1,0,0}}));
        e.AddChange(new Int2(0,4),SlavekTile.Boat, null);
        e.AddChange(new Int2(0,4),SlavekTile.Water, new CoefficientMap(s, new double[,] 
{{1,0,1,0,0},
{0,0,0,0,0},
{1,1,0,0,0},
{0,0,0,0,0},
{1,1,1,0,0}}));
        e.AddChange(new Int2(1,0),SlavekTile.Boat, null);
        e.AddChange(new Int2(1,0),SlavekTile.Water, new CoefficientMap(s, new double[,] 
{{1,0,1,0,0},
{0,0,0,0,0},
{1,1,0,0,0},
{0,0,0,0,0},
{1,1,1,0,0}}));
        e.AddChange(new Int2(1,1),SlavekTile.Boat, null);
        e.AddChange(new Int2(1,1),SlavekTile.Water, new CoefficientMap(s, new double[,] 
{{1,0,1,0,0},
{0,0,0,0,0},
{1,1,0,0,0},
{0,0,0,0,0},
{1,1,1,0,0}}));
        e.AddChange(new Int2(1,2),SlavekTile.Boat, null);
        e.AddChange(new Int2(1,2),SlavekTile.Water, new CoefficientMap(s, new double[,] 
{{1,0,1,0,0},
{0,0,0,0,0},
{1,1,0,0,0},
{0,0,0,0,0},
{1,1,1,0,0}}));
        e.AddChange(new Int2(1,3),SlavekTile.Boat, null);
        e.AddChange(new Int2(1,3),SlavekTile.Water, new CoefficientMap(s, new double[,] 
{{1,0,1,0,0},
{0,0,0,0,0},
{1,1,0,0,0},
{0,0,0,0,0},
{1,1,1,0,0}}));
        e.AddChange(new Int2(1,4),SlavekTile.Boat, null);
        e.AddChange(new Int2(1,4),SlavekTile.Water, new CoefficientMap(s, new double[,] 
{{1,0,1,0,0},
{0,0,0,0,0},
{1,1,0,0,0},
{0,0,0,0,0},
{1,1,1,0,0}}));
        e.AddChange(new Int2(2,0),SlavekTile.Boat, new CoefficientMap(s, new double[,] 
{{1,0,1,0,0},
{0,0,0,0,0},
{1,1,0,0,0},
{0,0,0,0,0},
{1,1,1,0,0}}));
        e.AddChange(new Int2(2,0),SlavekTile.Water, null);
        e.AddChange(new Int2(2,1),SlavekTile.Boat, new CoefficientMap(s, new double[,] 
{{1,0,1,0,0},
{0,0,0,0,0},
{1,1,0,0,0},
{0,0,0,0,0},
{1,1,1,0,0}}));
        e.AddChange(new Int2(2,1),SlavekTile.Water, null);
        e.AddChange(new Int2(2,2),SlavekTile.Boat, null);
        e.AddChange(new Int2(2,2),SlavekTile.Water, new CoefficientMap(s, new double[,] 
{{1,0,1,0,0},
{0,0,0,0,0},
{1,1,0,0,0},
{0,0,0,0,0},
{1,1,1,0,0}}));
        e.AddChange(new Int2(2,3),SlavekTile.Boat, null);
        e.AddChange(new Int2(2,3),SlavekTile.Water, new CoefficientMap(s, new double[,] 
{{1,0,1,0,0},
{0,0,0,0,0},
{1,1,0,0,0},
{0,0,0,0,0},
{1,1,1,0,0}}));
        e.AddChange(new Int2(2,4),SlavekTile.Boat, null);
        e.AddChange(new Int2(2,4),SlavekTile.Water, new CoefficientMap(s, new double[,] 
{{1,0,1,0,0},
{0,0,0,0,0},
{1,1,0,0,0},
{0,0,0,0,0},
{1,1,1,0,0}}));
        e.AddChange(new Int2(3,0),SlavekTile.Boat, null);
        e.AddChange(new Int2(3,0),SlavekTile.Water, new CoefficientMap(s, new double[,] 
{{1,0,1,0,0},
{0,0,0,0,0},
{1,1,0,0,0},
{0,0,0,0,0},
{1,1,1,0,0}}));
        e.AddChange(new Int2(3,1),SlavekTile.Boat, null);
        e.AddChange(new Int2(3,1),SlavekTile.Water, new CoefficientMap(s, new double[,] 
{{1,0,1,0,0},
{0,0,0,0,0},
{1,1,0,0,0},
{0,0,0,0,0},
{1,1,1,0,0}}));
        e.AddChange(new Int2(3,2),SlavekTile.Boat, null);
        e.AddChange(new Int2(3,2),SlavekTile.Water, new CoefficientMap(s, new double[,] 
{{1,0,1,0,0},
{0,0,0,0,0},
{1,1,0,0,0},
{0,0,0,0,0},
{1,1,1,0,0}}));
        e.AddChange(new Int2(3,3),SlavekTile.Boat, null);
        e.AddChange(new Int2(3,3),SlavekTile.Water, new CoefficientMap(s, new double[,] 
{{1,0,1,0,0},
{0,0,0,0,0},
{1,1,0,0,0},
{0,0,0,0,0},
{1,1,1,0,0}}));
        e.AddChange(new Int2(3,4),SlavekTile.Boat, null);
        e.AddChange(new Int2(3,4),SlavekTile.Water, new CoefficientMap(s, new double[,] 
{{1,0,1,0,0},
{0,0,0,0,0},
{1,1,0,0,0},
{0,0,0,0,0},
{1,1,1,0,0}}));
        e.AddChange(new Int2(4,0),SlavekTile.Boat, new CoefficientMap(s, new double[,] 
{{1,0,1,0,0},
{0,0,0,0,0},
{1,1,0,0,0},
{0,0,0,0,0},
{1,1,1,0,0}}));
        e.AddChange(new Int2(4,0),SlavekTile.Water, null);
        e.AddChange(new Int2(4,1),SlavekTile.Boat, new CoefficientMap(s, new double[,] 
{{1,0,1,0,0},
{0,0,0,0,0},
{1,1,0,0,0},
{0,0,0,0,0},
{1,1,1,0,0}}));
        e.AddChange(new Int2(4,1),SlavekTile.Water, null);
        e.AddChange(new Int2(4,2),SlavekTile.Boat, new CoefficientMap(s, new double[,] 
{{1,0,1,0,0},
{0,0,0,0,0},
{1,1,0,0,0},
{0,0,0,0,0},
{1,1,1,0,0}}));
        e.AddChange(new Int2(4,2),SlavekTile.Water, null);
        e.AddChange(new Int2(4,3),SlavekTile.Boat, null);
        e.AddChange(new Int2(4,3),SlavekTile.Water, new CoefficientMap(s, new double[,] 
{{1,0,1,0,0},
{0,0,0,0,0},
{1,1,0,0,0},
{0,0,0,0,0},
{1,1,1,0,0}}));
        e.AddChange(new Int2(4,4),SlavekTile.Boat, null);
        e.AddChange(new Int2(4,4),SlavekTile.Water, new CoefficientMap(s, new double[,] 
{{1,0,1,0,0},
{0,0,0,0,0},
{1,1,0,0,0},
{0,0,0,0,0},
{1,1,1,0,0}}));
        return e;
    }
}
