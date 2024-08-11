using BattleShipEngine;

namespace BattleShipStrategies.Slavek.AI;

public partial record struct PreparedMap
{
    public static PreparedMap MapSmaDef_MarKubMax()
    {
        GameSetting s = new GameSetting(10,10, new int[] {4,3,2,1});
        Int2[] boats = new Int2[20];
        boats[0] = new Int2(7, 7);
        boats[1] = new Int2(6, 7);
        boats[2] = new Int2(5, 7);
        boats[3] = new Int2(4, 7);
        boats[4] = new Int2(0, 7);
        boats[5] = new Int2(0, 6);
        boats[6] = new Int2(0, 5);
        boats[7] = new Int2(0, 3);
        boats[8] = new Int2(0, 2);
        boats[9] = new Int2(0, 1);
        boats[10] = new Int2(2, 1);
        boats[11] = new Int2(2, 2);
        boats[12] = new Int2(2, 5);
        boats[13] = new Int2(3, 5);
        boats[14] = new Int2(3, 9);
        boats[15] = new Int2(4, 9);
        boats[16] = new Int2(8, 3);
        boats[17] = new Int2(8, 9);
        boats[18] = new Int2(2, 7);
        boats[19] = new Int2(9, 6);
        return new PreparedMap(s, boats);
    }
}
