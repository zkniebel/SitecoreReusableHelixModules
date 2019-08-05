# Sitecore Reusable Helix Modules
A reusable set of Helix-compliant modules for Sitecore 8+. These modules can be used on any project, and they can be used together or separately. Note that some modules have dependencies (all valid), so it is important to remember to look at the those dependencies when you take only a subset of the modules.

--- 

## A note on serialization...
This project uses TDS for item serialization. I can easily pass along a Sitecore package containing the necessary items, per request (@zachary_kniebel on Sitecore Slack). However, even if your project doens't use TDS, you should be able to connect to the databases, push your items and then serialize them into Unicorn. If you don't have a TDS license, I recommend that you install the trial. If all else fails/your trial ran out, feel free to reach out to me and I'll send you a package with the items you need.

---

## Included Modules
The below are the modules that are currently included in the repo, and their descriptions:

---

### Foundation.SitecoreExtensions
The *Foundation.SitecoreExtensions* is a small module that is meant to be combinable with any *SitecoreExtensions* module that you already have in use in your solution. **The extensions included in this module are not simply generic extensions for use on any project** (I am a firm believer in [YAGNI](https://en.wikipedia.org/wiki/You_aren%27t_gonna_need_it)) but rather are common types/tools that extend Sitecore that other modules in the solution depend on. Currently, the project only contains the abstractions and required enum type for the custom *Disabler* implementation that I use in my code, especially though not exclusively in rules and event handlers. 

#### Highlights
- Very lean and should be easy to move logic from this module into your own *SitecoreExtensions* module 
- Custom Disablers abstraction and the required enum type for disabler state

---

### Foundation.Rules
The *Foundation.Rules* module provides the necessary base classes, abstractions, utilities and custom macros necessary to fully leverage the rules engine in your solution. Additionally, there are also some highly generic rules conditions and actions included in this module.

#### Highlights
- RuleManager Class
- Base Classes for Conditions and Actions
- Item Hierarchy Operator
- AddFromTemplateRulesProcessor
- Conditions
  - Item created from any branch
  - Item created from specific branch
  - Item hierarchy inherits template
- Actions
  - Create child item 
  - Set Field Value
  - Assign Workflow
  
#### Dependencies
- Foundation.SitecoreExtensions

---

### Feature.BranchPresets
A powerful module that adds a robust Branch Presets implementation, enabling designers/developers to create items with preset presentation and locally-scoped datasources as a branch template so that authors can create new pages/sites with a preconfigured experience. The Branch Presets module includes the rules logic to automagically relink local rendering datasources and field references (links, multilists, etc.), so that they all link to the items in the new instance of the branch. This module is vital for accelerating the content authoring experience, creating more effective content presentation governance (with regard to design), and drastically reducing the time required to add new sites to the solution.

#### Highlights
- Actions to relink datasources pointing to items in branch template
- Actions to relink field value references pointing to items in branch template 
- Preconfigured rules for relinking datasources and field value references to get Branch Presets working with minimal setup
- Supports relinking field value references for the following field types (all applicable types, excluding system types):
  - LinkField (General Link, etc.)
  - LookupField (Droplink, etc.)
  - ReferenceField (Droptree, etc.)
  - MultilistField (Treelist, etc.)
  
#### Dependencies
- Foundation.Rules

#### Setup/Testing Instructions
The following instructions were assembled to guide you in rapidly setting up and experimenting with Branch Presets.  

1. Build and publish the code and deploy the Sitecore items from the solution
2. Create a branch template if you don't already have one. For your first time using Branch Presets, it's recommended that you create a branch template that includes at least two pages that have one or more local datasources (i.e. datasources that live under the owning page item) each, so that you can witness the full power of the ~~dark side~~ Branch Presets. Note that you can also try putting your datasources elsewhere within the branch and they will still relink, so feel free to try that out too. Note that you can use the Experience Editor for branch templates too. 
3. Update the field values of the items in your branch templates as needed. If this is your first time playing with Branch Presets, I also recommend setting some link, reference, and multireference fields to point at items within the branch so that you can see that they'll be relinked for you, as well as the datasources. Also, if this is your first time using Branch Presents, I recommend adding a rendering that references a datasource that *does not* live within the branch and some field references to items that *do not* live within the branch so that you can see that they won't be modified.
4. Navigate to your _/sitecore/system/settings/rules/pipelines/addFromTemplate/rules/Branch Presets_ item (installed from the TDS projects included in the repo) and edit the rule to by setting the "[specific]" template referenced in the "and where the item's [descendants and self] has or inherits from the [specific] template" rule condition to your "Base Page" template (or whatever you call it). If you don't have a template like that (yikes!) then do whatever you think is best to make it work for your solution (e.g. a template that all page templates inherit)
5. At this point, everything should be in place and ready for you to begin testing, so start creating instances of your branch template and check out the datasources of your renderings and field references when you do. You should see that all references to items defined in the branch template will have been relinked to the new instances of those items.

---

### Feature.AuthoringGovernance
Currently, the Feature.AuthoringGovernance module contains only Sitecore rule items built out using the conditions and actions from the Foundation.Rules module. It is intended for more rules and functionality to be added to this module in the future. 

#### Highlights
- Rule that assigns a specific workflow to a newly created item based on the user's role
- Rule for adding child item whenever an item that inherits a specific template is added from a branch template (e.g. add local content folders on page item creation)

#### Dependencies
- Foundation.Rules (Content depends on the content and code from Foundation.Rules)

