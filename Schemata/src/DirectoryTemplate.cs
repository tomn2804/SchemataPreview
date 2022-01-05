﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Management.Automation;

namespace Schemata;

public sealed partial class DirectoryTemplate : Template<DirectoryModel>
{
    public DirectoryTemplate(IDictionary details)
        : base(details)
    {
    }

    protected override Blueprint ToBlueprint()
    {
        return new FileSystemTemplate(Details.SetItems(new[] { GetOnCreatingDetail(), GetOnMountedDetail(), GetOnMountingDetail() }));
    }

    private KeyValuePair<object, object> GetOnCreatingDetail()
    {
        EventHandler<Activity.ProcessingEventArgs> handler = (object? sender, Activity.ProcessingEventArgs args) =>
        {
            if (Details.TryGetValue(FileSystemTemplate.DetailOption.OnCreating, out object? onCreatingValue))
            {
                switch (onCreatingValue)
                {
                    case ScriptBlock scriptBlock:
                        scriptBlock.Invoke(sender, args);
                        break;

                    default:
                        ((EventHandler<Activity.ProcessingEventArgs>)onCreatingValue).Invoke(sender, args);
                        break;
                }
            }
            Node node = (Node)sender!;
            ((DirectoryModel)node.Model).Create();
        };
        return KeyValuePair.Create<object, object>(FileSystemTemplate.DetailOption.OnCreating, handler);
    }

    private KeyValuePair<object, object> GetOnMountedDetail()
    {
        EventHandler<Activity.ProcessedEventArgs> handler = (object? sender, Activity.ProcessedEventArgs args) =>
        {
            Node node = (Node)sender!;
            if (Details.TryGetValue(DetailOption.Children, out object? childrenValue))
            {
                IEnumerable<object>? children = childrenValue as IEnumerable<object>;
                if (children is null)
                {
                    children = new[] { childrenValue };
                }
                ((DirectoryModel)node.Model).Children.AddRange(children.Cast<Template>());
            }
            if (Details.TryGetValue(FileSystemTemplate.DetailOption.OnMounted, out object? onMountedValue))
            {
                switch (onMountedValue)
                {
                    case ScriptBlock scriptBlock:
                        scriptBlock.Invoke(sender, args);
                        break;

                    default:
                        ((EventHandler<Activity.ProcessedEventArgs>)onMountedValue).Invoke(sender, args);
                        break;
                }
            }
        };
        return KeyValuePair.Create<object, object>(FileSystemTemplate.DetailOption.OnMounted, handler);
    }

    private KeyValuePair<object, object> GetOnMountingDetail()
    {
        EventHandler<Activity.ProcessingEventArgs> handler = (object? sender, Activity.ProcessingEventArgs args) =>
        {
            if (Details.TryGetValue(FileSystemTemplate.DetailOption.OnMounting, out object? onMountingValue))
            {
                switch (onMountingValue)
                {
                    case ScriptBlock scriptBlock:
                        scriptBlock.Invoke(sender, args);
                        break;

                    default:
                        ((EventHandler<Activity.ProcessingEventArgs>)onMountingValue).Invoke(sender, args);
                        break;
                }
            }
            Node node = (Node)sender!;
            node.Invoke(node.Model.Activities[FileSystemTemplate.ActivityOption.Create]);
        };
        return KeyValuePair.Create<object, object>(FileSystemTemplate.DetailOption.OnMounting, handler);
    }
}
