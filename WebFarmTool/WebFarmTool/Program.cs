using System.Linq;
using Microsoft.Web.Administration;

namespace WebFarmTool
{
    class Program
    {
        static void Main(string[] args)
        {
            var program = new Program();
            // onlineServer というアドレスのサーバーをWebFarmに追加
            program.AddServer("webfarm-ctrl.cloudapp.net", "onlineServer");
            // onlineServer というアドレスのサーバーをWebFarmにOffline状態で追加
            program.AddServer("webfarm-ctrl.cloudapp.net", "offlineServer", false);
            // webfarm-node1.cloudapp.net サーバーを Online状態にする
            program.UpdateServerStatus("webfarm-ctrl.cloudapp.net", "webfarm-node1.cloudapp.net", true);
            // webfarm-node2.cloudapp.net サーバーを Offline状態にする
            program.UpdateServerStatus("webfarm-ctrl.cloudapp.net", "webfarm-node2.cloudapp.net", false);
        }

        void AddServer(string webFarmName, string serverName, bool isOnline = true)
        {
            using (var manager = new ServerManager())
            {
                // ApplicationHostの設定を取得
                var appHostConfig = manager.GetApplicationHostConfiguration();

                // WebFarmの設定リストから指定したWebFarm名の要素を取得
                var webFarmsSection = appHostConfig.GetSection("webFarms");
                var webFarmSection = webFarmsSection.GetCollection().First(e => e["name"] as string == webFarmName);
                
                // 子要素のserver要素のリストを取得
                var serversCollection = webFarmSection.GetCollection();
                // 新規のserver要素を作成して追加
                var newServer = serversCollection.CreateElement("server");
                newServer["address"] = serverName;
                newServer["enabled"] = isOnline;
                serversCollection.Add(newServer);

                manager.CommitChanges();
            }
        }


        void UpdateServerStatus(string webFarmName, string serverName, bool isOnline)
        {
            using (var manager = new ServerManager())
            {
                // ApplicationHostの設定を取得
                var appHostConfig = manager.GetApplicationHostConfiguration();

                // WebFarmの設定リストから指定したWebFarm名の要素を取得
                var webFarmsSection = appHostConfig.GetSection("webFarms");
                var webFarmSection = webFarmsSection.GetCollection().First(e => e["name"] as string == webFarmName);
                
                // 子要素のserver要素のリストを取得
                var serversCollection = webFarmSection.GetCollection();
                // 指定したサーバー名の要素を取得して属性を更新
                var server = serversCollection.First(e => e["address"] as string == serverName);
                server["enabled"] = isOnline;

                manager.CommitChanges();
            }
        }
    }
}
