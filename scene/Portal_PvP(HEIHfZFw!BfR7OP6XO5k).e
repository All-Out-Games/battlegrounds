13
124554051585
31069919089008 1716413396906995300
{
  "name": "Portal_PvP",
  "local_enabled": true,
  "local_position": {
    "X": -7.2285013198852539,
    "Y": -8.1192588806152344
  },
  "local_rotation": 0,
  "local_scale": {
    "X": 0.3000000119209290,
    "Y": 0.3000000119209290
  },
  "previous_sibling": "46145610696738:1723586970968342300",
  "next_sibling": "36341083635957:1717793673228865000",
  "parent": "359905312717597:1716331881724713300",
  "spawn_as_networked_entity": true
},
{
  "cid": 1,
  "aoid": "31162563296502:1716413443303375800",
  "component_type": "Internal_Component",
  "internal_component_type": "Spine_Animator",
  "data": {
    "skeleton_data_asset": "environment/CentralHub/Portals/animation/BAT003_portals.spine",
    "ordered_skins": [
      "swords"
    ],
    "depth_offset": 3.6600000858306885,
    "skeleton_scale": {
      "X": 2.4000000953674316,
      "Y": 2.4000000953674316
    }
  }
},
{
  "cid": 3,
  "aoid": "31207523722928:1716413465819635300",
  "component_type": "Internal_Component",
  "internal_component_type": "Interactable",
  "data": {
    "text": "Teleport to PVP"
  }
},
{
  "cid": 2,
  "aoid": "366140754091523:1716484102667505400",
  "component_type": "Mono_Component",
  "mono_component_type": "ZoneTeleporter",
  "data": {
    "ChangeStatusTo": "Combat"
  }
}
