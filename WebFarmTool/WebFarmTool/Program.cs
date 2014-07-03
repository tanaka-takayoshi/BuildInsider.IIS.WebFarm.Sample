using System;
using System.Linq;
using Microsoft.Web.Administration;

namespace WebFarmTool
{
    class Program
    {
        static void Main(string[] args)
        {
            var program = new Program();
            // 連載9回のサンプル
            // onlineServer というアドレスのサーバーをWebFarmに追加
            program.AddServer("webfarm-ctrl.cloudapp.net", "onlineServer");
            // onlineServer というアドレスのサーバーをWebFarmにOffline状態で追加
            program.AddServer("webfarm-ctrl.cloudapp.net", "offlineServer", false);
            // webfarm-node1.cloudapp.net サーバーを Online状態にする
            program.UpdateServerStatus("webfarm-ctrl.cloudapp.net", "webfarm-node1.cloudapp.net", true);
            // webfarm-node2.cloudapp.net サーバーを Offline状態にする
            program.UpdateServerStatus("webfarm-ctrl.cloudapp.net", "webfarm-node2.cloudapp.net", false);

            // 連載10回のサンプル
            program.UpdateHelathCheck("webfarm-ctrl.cloudapp.net", TimeSpan.FromMinutes(1), TimeSpan.FromSeconds(30), 2);
            program.UpdateProxySetting("webfarm-ctrl.cloudapp.net", TimeSpan.FromSeconds(10), 512, 8192);
            program.UpdateClientAffinity("webfarm-ctrl.cloudapp.net", true, "MyARRAffinity");
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

        void UpdateHelathCheck(string webFarmName, TimeSpan interval, TimeSpan timeout, int minServers)
        {
            using (var manager = new ServerManager())
            {
                // ApplicationHostの設定を取得
                var appHostConfig = manager.GetApplicationHostConfiguration();

                // WebFarmの設定リストから指定したWebFarm名の要素を取得
                var webFarmsSection = appHostConfig.GetSection("webFarms");
                var webFarmSection = webFarmsSection.GetCollection().First(e => e["name"] as string == webFarmName);

                // applicationRequestRouting->healthCheck要素を取得して設定
                var applicationRequestRouting = webFarmSection.GetChildElement("applicationRequestRouting");
                var healthCheck = applicationRequestRouting.GetChildElement("healthCheck");
                healthCheck["interval"] = interval; 
                healthCheck["timeout"] = timeout;
                healthCheck["minServers"] = minServers;
                manager.CommitChanges();
            }
        }

        void UpdateProxySetting(string webFarmName, TimeSpan timeout, int minResponseBuffer, int responseBufferLimit)
        {
            using (var manager = new ServerManager())
            {
                // ApplicationHostの設定を取得
                var appHostConfig = manager.GetApplicationHostConfiguration();

                // WebFarmの設定リストから指定したWebFarm名の要素を取得
                var webFarmsSection = appHostConfig.GetSection("webFarms");
                var webFarmSection = webFarmsSection.GetCollection().First(e => e["name"] as string == webFarmName);

                // applicationRequestRouting->protocol要素を取得して設定
                var applicationRequestRouting = webFarmSection.GetChildElement("applicationRequestRouting");
                var protocol = applicationRequestRouting.GetChildElement("protocol");
                protocol["timeout"] = timeout;
                protocol["minResponseBuffer"] = minResponseBuffer;
                protocol["responseBufferLimit"] = responseBufferLimit;
                manager.CommitChanges();
            }
        }

        void UpdateClientAffinity(string webFarmName, bool use, string cookieName)
        {
            using (var manager = new ServerManager())
            {
                // ApplicationHostの設定を取得
                var appHostConfig = manager.GetApplicationHostConfiguration();

                // WebFarmの設定リストから指定したWebFarm名の要素を取得
                var webFarmsSection = appHostConfig.GetSection("webFarms");
                var webFarmSection = webFarmsSection.GetCollection().First(e => e["name"] as string == webFarmName);

                // applicationRequestRouting->affinity要素を取得して設定
                var applicationRequestRouting = webFarmSection.GetChildElement("applicationRequestRouting");
                var affinity = applicationRequestRouting.GetChildElement("affinity");
                affinity["useCookie"] = use;
                affinity["cookieName"] = cookieName;
                manager.CommitChanges();
            }
        }
    }
}
