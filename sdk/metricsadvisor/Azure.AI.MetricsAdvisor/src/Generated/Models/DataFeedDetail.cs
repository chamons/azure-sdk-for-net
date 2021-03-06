// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

// <auto-generated/>

#nullable disable

using System;
using System.Collections.Generic;
using System.Linq;
using Azure.Core;

namespace Azure.AI.MetricsAdvisor.Models
{
    /// <summary> The DataFeedDetail. </summary>
    internal partial class DataFeedDetail
    {

        /// <summary> data source type. </summary>
        internal DataFeedSourceType DataSourceType { get; set; }
        /// <summary> data feed name. </summary>
        public string DataFeedName { get; set; }
        /// <summary> data feed description. </summary>
        public string DataFeedDescription { get; set; }
        /// <summary> granularity of the time series. </summary>
        public DataFeedGranularityType GranularityName { get; set; }
        /// <summary> if granularity is custom,it is required. </summary>
        public int? GranularityAmount { get; set; }
        /// <summary> measure list. </summary>
        public IList<DataFeedMetric> Metrics { get; }
        /// <summary> user-defined timestamp column. if timestampColumn is null, start time of every time slice will be used as default value. </summary>
        public string TimestampColumn { get; set; }
        /// <summary> ingestion start time. </summary>
        public DateTimeOffset DataStartFrom { get; set; }
        /// <summary> the time that the beginning of data ingestion task will delay for every data slice according to this offset. </summary>
        public long? StartOffsetInSeconds { get; set; }
        /// <summary> the max concurrency of data ingestion queries against user data source. 0 means no limitation. </summary>
        public int? MaxConcurrency { get; set; }
        /// <summary> the min retry interval for failed data ingestion tasks. </summary>
        public long? MinRetryIntervalInSeconds { get; set; }
        /// <summary> stop retry data ingestion after the data slice first schedule time in seconds. </summary>
        public long? StopRetryAfterInSeconds { get; set; }
        /// <summary> mark if the data feed need rollup. </summary>
        public DataFeedRollupType? NeedRollup { get; set; }
        /// <summary> roll up method. </summary>
        public DataFeedAutoRollupMethod? RollUpMethod { get; set; }
        /// <summary> the identification value for the row of calculated all-up value. </summary>
        public string AllUpIdentification { get; set; }
        /// <summary> the type of fill missing point for anomaly detection. </summary>
        public DataFeedMissingDataPointFillType? FillMissingPointType { get; set; }
        /// <summary> the value of fill missing point for anomaly detection. </summary>
        public double? FillMissingPointValue { get; set; }
        /// <summary> data feed access mode, default is Private. </summary>
        public DataFeedAccessMode? ViewMode { get; set; }
        /// <summary> the query user is one of data feed administrator or not. </summary>
        public bool? IsAdmin { get; }
        /// <summary> data feed creator. </summary>
        public string Creator { get; }
        /// <summary> data feed status. </summary>
        public DataFeedStatus? Status { get; }
        /// <summary> data feed created time. </summary>
        public DateTimeOffset? CreatedTime { get; }
        /// <summary> action link for alert. </summary>
        public string ActionLinkTemplate { get; set; }
    }
}
