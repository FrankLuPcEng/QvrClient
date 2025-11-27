using QvrProClient.Models;

namespace QvrProClient.WpfTester.ViewModels;

public class CameraViewModel
{
    public CameraViewModel(QvrCameraInfo camera)
    {
        CameraId = camera.Id;
        Name = string.IsNullOrWhiteSpace(camera.Name) ? "(unnamed)" : camera.Name;
        ChannelId = camera.Channel ?? string.Empty;
        IpOrUrl = !string.IsNullOrWhiteSpace(camera.Url)
            ? camera.Url!
            : !string.IsNullOrWhiteSpace(camera.Ip)
                ? camera.Ip!
                : string.Empty;
    }

    public string CameraId { get; }

    public string Name { get; }

    public string ChannelId { get; }

    public string IpOrUrl { get; }
}
