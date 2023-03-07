using System;
using System.Threading.Tasks;
using UnityEngine;
//using UnityEngine.AddressableAssets;

namespace GenerateMap
{
    class MapDrawer
    {
        private TileUnit[,,] tileUnits;

        public MapDrawer()
        {
        }

        internal async Task DrawMap(TileDetail[,,] tileDetails)
        {
            int xNum = tileDetails.GetLength(0);
            int yNum = tileDetails.GetLength(1);
            int zNum = tileDetails.GetLength(2);

            tileUnits = new TileUnit[xNum, yNum, zNum];

            for (int x = 0; x < xNum; ++x)
                for (int y = 0; y < yNum; ++y)
                    for (int z = 0; z < zNum; ++z)
                    {
                        TileDetail detail = tileDetails[x, y, z];
                        if(detail.tileType != eTileType.None)
                        {
                            GameObject go = GameObject.CreatePrimitive(PrimitiveType.Cube);
                            TileUnit tileUnit = go.AddComponent<TileUnit>();
                            tileUnit.TileType = detail.tileType;
                            tileUnit.Position = new Vector3(x, y, z);
                            tileUnit.SetMaterial(await GetMaterialAsync(detail.tileType));
                        }
                    }
        }

        private async Task<Material> GetMaterialAsync(eTileType tileType)
        {
            TaskCompletionSource<Material> tcs=new TaskCompletionSource<Material>(TaskCreationOptions.RunContinuationsAsynchronously);
            string matn="Grass.mat";
            switch (tileType)
            {
                case eTileType.Earth:
                    matn="Grass.mat";
                    break;
                case eTileType.Stone:
                    matn="Grey Stones.mat";
                    break;
                case eTileType.Ocean:
                    matn="Water Deep Blue.mat";
                    break;
                case eTileType.Lake:
                    matn="Water Light Blue.mat";
                    break;
                case eTileType.Tree:
                    matn="Sandy.mat";
                    break;
                case eTileType.Sand:
                    matn="Sandy Orange.mat";
                    break;
            }  
            tcs.SetResult(Resources.Load<Material>("Assets/Addressible/Cubes/Materials/"+matn));
            return await tcs.Task;            
        }
    }
}