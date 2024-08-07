using BattleShipEngine;

namespace BattleShipStrategies.Slavek.AI;

public partial record struct PreparedMap
{
    public static PreparedMap MapNem_DefMar()
    {
        GameSetting s = new GameSetting(10,10, new int[] {4,3,2,1});
        Int2[] boats = new Int2[20];
        boats[0] = new Int2(9, 2);
        boats[1] = new Int2(9, 3);
        boats[2] = new Int2(9, 4);
        boats[3] = new Int2(9, 5);
        boats[4] = new Int2(7, 2);
        boats[5] = new Int2(7, 3);
        boats[6] = new Int2(7, 4);
        boats[7] = new Int2(0, 5);
        boats[8] = new Int2(0, 6);
        boats[9] = new Int2(0, 7);
        boats[10] = new Int2(4, 2);
        boats[11] = new Int2(4, 1);
        boats[12] = new Int2(7, 9);
        boats[13] = new Int2(7, 8);
        boats[14] = new Int2(0, 9);
        boats[15] = new Int2(1, 9);
        boats[16] = new Int2(2, 7);
        boats[17] = new Int2(9, 8);
        boats[18] = new Int2(7, 6);
        boats[19] = new Int2(2, 3);
        return new PreparedMap(s, boats);
    }
}
