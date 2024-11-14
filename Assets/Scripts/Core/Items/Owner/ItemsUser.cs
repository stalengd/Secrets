using Anomalus.Player;
using VContainer;

namespace Anomalus.Items.Owner
{
    public sealed class ItemsUser
    {
        [Inject] private readonly PlayerSpawner _playerSpawner;

        // TODO: Different references to players components like HP, so item systems can affect them on use

        public void EquipItem(ItemStack item)
        {

        }

        public void UnequipItem(ItemStack item)
        {

        }
    }
}
