13
107374182401
46145610696738 1723586970968342300
{
  "name": "Portal_AFK",
  "local_enabled": true,
  "local_position": {
    "X": -7.1815843582153320,
    "Y": -8.1192588806152344
  },
  "local_rotation": 0,
  "local_scale": {
    "X": 0.3000000119209290,
    "Y": 0.3000000119209290
  },
  "previous_sibling": "198337835043157:1722286871474699800",
  "parent": "359905312717597:1716331881724713300",
  "spawn_as_networked_entity": true
},
{
  "cid": 1,
  "aoid": "46145610892819:1723586970968420000",
  "component_type": "Internal_Component",
  "internal_component_type": "Spine_Animator",
  "data": {
    "skeleton_data_asset": "environment/CentralHub/Portals/animation/BAT003_portals.spine",
    "ordered_skins": [
      "hourglass"
    ],
    "depth_offset": 3.6600000858306885,
    "skeleton_scale": {
      "X": 2.4000000953674316,
      "Y": 2.4000000953674316
    },
    "mask_in_shadow": false
  }
},
{
  "cid": 3,
  "aoid": "46145611064317:1723586970968488900",
  "component_type": "Internal_Component",
  "internal_component_type": "Interactable",
  "data": {
    "prompt_offset": {
      "X": 0,
      "Y": 1
    },
    "text": "Teleport to AFK",
    "hold_text": "",
    "radius": 2,
    "required_hold_time": 0.6000000238418579
  }
},
{
  "cid": 2,
  "aoid": "46145611255728:1723586970968565600",
  "component_type": "Mono_Component",
  "mono_component_type": "ZoneTeleporter",
  "data": {
    "ChangeStatusTo": "AFK"
  }
}
