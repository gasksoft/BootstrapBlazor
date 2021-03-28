﻿// Copyright (c) Argo Zhang (argo@163.com). All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.
// Website: https://www.blazor.zone or https://argozhang.github.io/

using BootstrapBlazor.Localization.Json;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Globalization;
using System.Linq;

namespace BootstrapBlazor.Components
{
    /// <summary>
    /// IValidator 实现类基类
    /// </summary>
    public abstract class ValidatorBase : IValidator
    {
        /// <summary>
        /// 获得/设置 错误描述信息
        /// </summary>
        public string? ErrorMessage { get; set; }

        /// <summary>
        /// 数据验证方法
        /// </summary>
        /// <param name="propertyValue"></param>
        /// <param name="context"></param>
        /// <param name="results"></param>
        public abstract void Validate(object? propertyValue, ValidationContext context, List<ValidationResult> results);

        /// <summary>
        /// 获得当前验证规则资源文件中 Key 格式
        /// </summary>
        /// <returns></returns>
        protected virtual string GetRuleKey() => GetType().Name.Split(".").Last().Replace("Validator", "");

        /// <summary>
        /// 通过资源文件获取 ErrorMessage 方法
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        protected virtual string? GetLocalizerErrorMessage(ValidationContext context)
        {
            var errorMesssage = ErrorMessage;
            if (!string.IsNullOrEmpty(context.MemberName) && !string.IsNullOrEmpty(errorMesssage))
            {
                // 查找 resx 资源文件中的 ErrorMessage
                var memberName = context.MemberName;

                var isResx = false;
                var resxType = ServiceProviderHelper.ServiceProvider.GetRequiredService<IOptions<JsonLocalizationOptions>>().Value.ResourceManagerStringLocalizerType;
                if (resxType != null && JsonStringLocalizerFactory.TryGetLocalizerString(resxType, errorMesssage, out var resx))
                {
                    errorMesssage = resx;
                    isResx = true;
                }

                if (!isResx && JsonStringLocalizerFactory.TryGetLocalizerString(context.ObjectType, $"{memberName}.{GetRuleKey()}", out var msg))
                {
                    errorMesssage = msg;
                }

                if (!string.IsNullOrEmpty(errorMesssage))
                {
                    var displayName = new FieldIdentifier(context.ObjectInstance, context.MemberName).GetDisplayName();
                    errorMesssage = string.Format(CultureInfo.CurrentCulture, errorMesssage, displayName ?? memberName);
                }
            }
            return errorMesssage;
        }
    }
}
