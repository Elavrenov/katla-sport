namespace KatlaSport.Services.HiveManagement
{
    /// <summary>
    /// upd
    /// </summary>
    public class UpdateHiveSectionRequest
    {
        /// <summary>
        /// Gets or sets the name.
        /// </summary>
        /// <value>
        /// The name.
        /// </value>
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the code.
        /// </summary>
        /// <value>
        /// The code.
        /// </value>
        public string Code { get; set; }

        /// <summary>
        /// Gets or sets the hive identifier.
        /// </summary>
        /// <value>
        /// The hive identifier.
        /// </value>
        public int StoreHiveId { get; set; }
    }
}
