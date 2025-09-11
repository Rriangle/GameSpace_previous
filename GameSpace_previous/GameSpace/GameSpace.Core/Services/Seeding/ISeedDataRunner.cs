using System.Threading.Tasks;

namespace GameSpace.Core.Services.Seeding
{
    /// <summary>
    /// �ؤl�ƾڹB�澹����
    /// </summary>
    public interface ISeedDataRunner
    {
        /// <summary>
        /// ����ؤl�ƾڪ�l��
        /// </summary>
        Task SeedAsync();
        
        /// <summary>
        /// �ˬd�O�_�ݭn����ؤl�ƾ�
        /// </summary>
        Task<bool> NeedsSeedingAsync();
    }
}
