using System;
using System.Linq;
using System.Runtime.Serialization;
using System.Reflection;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace FlowerFarmTaskManagementSystem.Common
{
    public static class EnumProductFieldStatusExtensions
    {
        public static string GetEnumMemberValue(this Enum value)
        {
            var type = value.GetType();
            var memberInfo = type.GetMember(value.ToString());
            if (memberInfo.Length > 0)
            {
                var attributes = memberInfo[0].GetCustomAttributes(typeof(EnumMemberAttribute), false);
                if (attributes.Length > 0)
                {
                    return ((EnumMemberAttribute)attributes[0]).Value;
                }
            }
            return value.ToString();
        }

        public static SelectList GetSelectList<TEnum>() where TEnum : Enum
        {
            var items = Enum.GetValues(typeof(TEnum))
                .Cast<TEnum>()
                .Select(e => new SelectListItem
                {
                    Value = e.ToString(), // Giá trị gửi đi là tên enum (ví dụ: "Growing")
                    Text = e.GetEnumMemberValue() // Hiển thị là tiếng Việt (ví dụ: "Đang phát triển")
                });
            return new SelectList(items, "Value", "Text");
        }
    }
}