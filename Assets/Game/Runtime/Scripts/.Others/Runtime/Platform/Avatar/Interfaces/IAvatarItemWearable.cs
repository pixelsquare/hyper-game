using System.Threading.Tasks;

namespace Kumu.Kulitan.Avatar
{
    public interface IAvatarItemWearable
    {
        public Task WearAvatarItem(IAvatarItemModelHandle colorizer);
        public Task RemoveAvatarItem(IAvatarItemModelHandle colorizer);
    }
}
