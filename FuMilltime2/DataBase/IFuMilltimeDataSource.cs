namespace FuMilltime2.DataBase
{
    using System;
    using System.Collections.Generic;

    using FuMilltime2.Controls;
    using FuMilltime2.Controls.ProjectInput;

    /// <summary>
    /// Data source for persistance of FUMilltime data.
    /// </summary>
    public interface IFuMilltimeDataSource
    {
        /// <summary>
        /// Gets the data for a specific date.
        /// </summary>
        /// <param name="date">The date.</param>
        /// <returns>The data stored for the specified date.</returns>
        IReadOnlyList<TimeRecord>? GetRecords();

        /// <summary>
        /// Sets the data for a specific date.
        /// </summary>
        /// <param name="date">The date.</param>
        /// <param name="data">The data.</param>
        void SaveRecords(IReadOnlyList<TimeRecord> data);
    }
}