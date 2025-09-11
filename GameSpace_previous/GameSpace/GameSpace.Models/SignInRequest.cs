using System.ComponentModel.DataAnnotations;

namespace GameSpace.Models
{
    /// <summary>
    /// ñ��ШD�ҫ� - �w�X�i�� Stage 3 �ݨD
    /// </summary>
    public class SignInRequest
    {
        /// <summary>
        /// �Τ�ID
        /// </summary>
        [Required]
        public int UserId { get; set; }

        /// <summary>
        /// �����ʪ��_
        /// </summary>
        [Required]
        public string IdempotencyKey { get; set; } = string.Empty;

        /// <summary>
        /// ñ�������]�C��B�C�g���^
        /// </summary>
        public string SignInType { get; set; } = "daily";

        // ===== Stage 3 �X�i��� =====

        /// <summary>
        /// ñ��ɶ��W�]�i��A�w�]����e�ɶ��^�]Stage 3 �s�W�^
        /// </summary>
        public DateTime? SignInTime { get; set; }
    }
}
