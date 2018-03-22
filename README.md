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

---

### Feature.AuthoringGovernance
Currently, the Feature.AuthoringGovernance module contains only Sitecore rule items built out using the conditions and actions from the Foundation.Rules module. It is intended for more rules and functionality to be added to this module in the future. 

#### Highlights
- Rule that assigns a specific workflow to a newly created item based on the user's role
- Rule for adding child item whenever an item that inherits a specific template is added from a branch template (e.g. add local content folders on page item creation)

#### Dependencies
- Foundation.Rules (Content depends on the content and code from Foundation.Rules)

