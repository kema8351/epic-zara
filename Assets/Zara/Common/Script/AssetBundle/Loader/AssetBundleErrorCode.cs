using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Zara.Common.ExAssetBundle
{
    public enum AssetBundleErrorCode
    {
        Unknown = 0,

        IO = 400,
        Network = 408,

        DuplicatePath = 520,
        UnknownErrorType = 521,
        MissingTaskDownloadFinished = 522,
        MissingTaskRetry = 523,
        MissingTaskRetryNetwork = 524,
        MissingTaskRetryIo = 525,

        MissingFinishedAction = 540,

        MissingLoadingAssetBundleRecord = 550,
        FailureToDecrypt = 551,
        FailureToGetAssetBundle = 552,
        FailureToRemoveLoadingAssetBundleRecord = 553,
        MissingTaskIdSetInCheckingCompletion = 554,
        MissingLoadingTask = 555,
        MissingAssetEntryRecord = 556,
        MissingAssetBundleName = 557,
        FailureToRemoveTaskId = 558,
        MissingTaskIdSetInTermination = 559,
        FailureToRemoveLoadingTaskIdSet = 560,
        MissingLoadedAssetBundle = 561,
        FailureToRemoveLoadedAssetBundle = 562,
        FailureToRemoveLoadingTask = 563,

        LoadedTable = 570,
        LoadingTable = 571,
        FailureToDeleteTable = 572,
        TableNotFound = 573,
        EntryKeyNotFound = 574,
        InvalidType = 575,

        NullAssetEntry = 580,
        NullAssetBundleTable = 581,
        DestroyedInLoadingTable = 582,
        DestroyedInClearance = 583,
        DestroyedInPreloading = 584,
        MissingRetryAction = 585,
    }
}