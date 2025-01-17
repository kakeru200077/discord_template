﻿using System.Text;

namespace discord_template
{
    internal class CommandSender
    {
        public readonly string[] m_CommandList;
        public readonly Ids m_Ids;

        public CommandSender(string? directoryPath, Ids ids)
        {
            if (directoryPath.IsNullOrEmpty()) { throw new Exception($"{nameof(directoryPath)}が不正です。\nnullもしくは空白です。"); }
            //ファイル一覧を取得
            if (!Directory.Exists(directoryPath)) { throw new Exception($"指定されたパス{directoryPath}は存在しません。"); }
            m_CommandList = Directory.GetFiles(directoryPath, "*.json");
            if (m_CommandList.Length <= 0) { throw new Exception("指定されたパス内にjsonファイルが存在しませんでした。"); }

            if (ids == null) { throw new ArgumentNullException("id"); }
            m_Ids = ids;
        }

        public void RequestSender()
        {
            if (m_CommandList == null) { throw new NullReferenceException($"{nameof(m_CommandList)}が不正です。\nnullもしくは空白です。"); }

            HttpRequestMessage[] requests = GetHeader();

            foreach (HttpRequestMessage request in requests) {
                foreach (string jsonCommand in m_CommandList) {
                    //getHaderとかいろいろ使っていろいろする
                }
            }
        }
        private HttpRequestMessage[] GetHeader()
        {
            if (m_Ids == null) { throw new NullReferenceException("ids"); }
            if (m_Ids.m_GuildIds == null) { throw new NullReferenceException("ids.guild_ids"); }
            HttpRequestMessage[] request = new HttpRequestMessage[m_Ids.m_GuildIds.Length];
            int idcount = 0;
            foreach (string guild_id in m_Ids.m_GuildIds) {
                if (guild_id.IsNullOrEmpty()) { throw new Exception("guild_idが不正です。\nguild_idがnull、もしくは空白です。"); }
                if (m_Ids.m_ApplicationId.IsNullOrEmpty()) { throw new Exception("application_idが不正です。\napplication_idがnull、もしくは空白です。"); }
                if (m_Ids.m_Token.IsNullOrEmpty()) { throw new Exception("Tokenが不正です。\nTokenがnull、もしくは空白です。"); }

                string url = "https://discord.com/api/v8/applications/" + m_Ids.m_ApplicationId + "/guilds/" + guild_id + "/commands";
                UriBuilder builder = new UriBuilder(new Uri(url));
                request[idcount] = new HttpRequestMessage(HttpMethod.Post, builder.Uri);
                request[idcount].Headers.Add("Authorization", "Bot " + m_Ids.m_Token);

                idcount++;
            }

            return request;
        }
        private HttpRequestMessage RequestContentBuilder(HttpRequestMessage requestMessage, string json_command)
        {
            //渡されたjson形式のコマンド情報をコンテンツに設定する
            if (json_command.IsNullOrEmpty()) { throw new Exception("json_commandが不正です。\njson_commandがnullもしくは空白です。\n"); }
            requestMessage.Content = new StringContent(json_command, Encoding.UTF8, "application/json");

            return requestMessage;
        }
    }
}
