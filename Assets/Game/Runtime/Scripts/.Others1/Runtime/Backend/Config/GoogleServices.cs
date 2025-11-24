using System.Collections.Generic;

namespace Santelmo.Rinsurv.Backend
{
    public class GoogleServices
    {
        public ProjectInfo project_info { get; set; }
        public List<Client> client { get; set; }
        public string configuration_version { get; set; }
    }

    public class AndroidClientInfo
    {
        public string package_name { get; set; }
    }

    public class ApiKey
    {
        public string current_key { get; set; }
    }

    public class AppinviteService
    {
        public List<OtherPlatformOauthClient> other_platform_oauth_client { get; set; }
    }

    public class Client
    {
        public ClientInfo client_info { get; set; }
        public List<OauthClient> oauth_client { get; set; }
        public List<ApiKey> api_key { get; set; }
        public Services services { get; set; }
    }

    public class ClientInfo
    {
        public string mobilesdk_app_id { get; set; }
        public AndroidClientInfo android_client_info { get; set; }
    }

    public class IosInfo
    {
        public string bundle_id { get; set; }
    }

    public class OauthClient
    {
        public string client_id { get; set; }
        public int client_type { get; set; }
    }

    public class OtherPlatformOauthClient
    {
        public string client_id { get; set; }
        public int client_type { get; set; }
        public IosInfo ios_info { get; set; }
    }

    public class ProjectInfo
    {
        public string project_number { get; set; }
        public string project_id { get; set; }
        public string storage_bucket { get; set; }
    }

    public class Services
    {
        public AppinviteService appinvite_service { get; set; }
    }
}
