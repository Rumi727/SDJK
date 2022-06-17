namespace SCKRM
{
    public interface IRemoveable
    {
        public bool isRemoved { get; }

        /// <summary>
        /// Object Remove
        /// </summary>
        /// <returns>Is Remove Success</returns>
        bool Remove();
    }

    public interface IRemoveableForce : IRemoveable
    {
        /// <summary>
        /// Object Remove
        /// </summary>
        /// <returns>Is Remove Success</returns>
        bool Remove(bool force);
    }
}
