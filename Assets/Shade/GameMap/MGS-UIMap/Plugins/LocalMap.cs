﻿/*************************************************************************
 *  Copyright © 2016-2018 Mogoson. All rights reserved.
 *------------------------------------------------------------------------
 *  File         :  LocalMap.cs
 *  Description  :  Define local map.
 *------------------------------------------------------------------------
 *  Author       :  Mogoson
 *  Version      :  0.1.0
 *  Date         :  3/8/2018
 *  Description  :  Initial development version.
 *************************************************************************/

using UnityEngine;

namespace MGS.UIMap
{
    /// <summary>
    /// Local map.
    /// </summary>
    [AddComponentMenu("MGS/UIMap/LocalMap")]
    public class LocalMap : GlobalMap
    {
        #region Protected Method
        /// <summary>
        /// Component update.
        /// </summary>
        protected override void Update()
        {
            //Update flags position and rotation.
            base.Update();
            UpdateMapPos();
        }

        /// <summary>
        /// Update map's position.
        /// </summary>
        protected void UpdateMapPos()
        {
            rectTrans.anchoredPosition = -GetMappingPos(dynamicFlags[0]);
        }
        #endregion
    }
}