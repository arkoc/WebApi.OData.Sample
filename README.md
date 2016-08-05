## WebApi.OData.Sample

#### `WebApi.OData.Sample` is project to reuse it as a base for future `WebApi OData` projects.
What is workflow of developing `WebAPI OData` with this sample ?

* Download Sample Solution and Open It
* Define your models and inherit from `ModelBase`
* Define your controllers `{entity_name} + "s" + Controller` and inherit them from BaseODataController<TEntity> with specifying entity type
* Define your `OData` `EntitySet`-s in `WebApiConfig` file.

You can override anything you want.

* You can change `ModelBase`
* You can override all actions in your entity controller
* You can just delete all files  and start from scratch.

Now you have fully working `OData v4 WebAPI`. Have fun.

Here is a small blog post about this sample:

[Oh, oh, oh Data ? No, OData!](http://arkoc.github.io/Oh-oh-oh-Data-No-OData/)
