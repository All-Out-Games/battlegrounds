13
292057776129
32182412942337 1732944016802780400
{
  "name": "Portal_Spectator",
  "local_enabled": true,
  "local_position": {
    "X": 3.8745307922363281,
    "Y": -8.1192588806152344
  },
  "local_rotation": 0,
  "local_scale": {
    "X": 0.3000000119209290,
    "Y": 0.3000000119209290
  },
  "previous_sibling": "31069919089008:1716413396906995300",
  "next_sibling": "36341083635957:1717793673228865000",
  "parent": "359905312717597:1716331881724713300",
  "spawn_as_networked_entity": true
},
{
  "cid": 1,
  "aoid": "32182413093956:1732944016802840400",
  "component_type": "Internal_Component",
  "internal_component_type": "Spine_Animator",
  "data": {
    "skeleton_data_asset": "environment/CentralHub/Portals/animation/BAT003_portals.spine",
    "ordered_skins": [
      "eye"
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
  "aoid": "32182413180204:1732944016802874900",
  "component_type": "Internal_Component",
  "internal_component_type": "Interactable",
  "data": {
    "text": "Spectral Spawn"
  }
},
{
  "cid": 2,
  "aoid": "32182413227709:1732944016802894000",
  "component_type": "Mono_Component",
  "mono_component_type": "ZoneTeleporter",
  "data": {
    "ChangeStatusTo": "Spectator"
  }
}
