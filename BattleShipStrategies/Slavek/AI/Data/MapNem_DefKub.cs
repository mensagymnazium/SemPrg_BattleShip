using BattleShipEngine;

namespace BattleShipStrategies.Slavek.AI;

public partial record struct PreparedMap
{
    public static PreparedMap MapNem_DefKub()
    {
        GameSetting s = new GameSetting(10,10, new int[] {4,3,2,1});
        Int2[] boats = new Int2[20];
        boats[0] = new Int2(5, 3);
        boats[1] = new Int2(5, 4);
        boats[2] = new Int2(5, 5);
        boats[3] = new Int2(5, 6);
        boats[4] = new Int2(0, 8);
        boats[5] = new Int2(0, 7);
        boats[6] = new Int2(0, 6);
        boats[7] = new Int2(7, 2);
        boats[8] = new Int2(8, 2);
        boats[9] = new Int2(9, 2);
        boats[10] = new Int2(8, 0);
        boats[11] = new Int2(7, 0);
        boats[12] = new Int2(2, 9);
        boats[13] = new Int2(2, 8);
        boats[14] = new Int2(3, 6);
        boats[15] = new Int2(2, 6);
        boats[16] = new Int2(4, 9);
        boats[17] = new Int2(6, 8);
        boats[18] = new Int2(4, 0);
        boats[19] = new Int2(2, 0);
        return new PreparedMap(s, boats);
    }
}
