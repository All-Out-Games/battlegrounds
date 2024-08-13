13
94489280513
54126695067805 1723590168519195200
{
  "name": "Portal",
  "local_enabled": true,
  "local_position": {
    "X": -6.9680557250976562,
    "Y": 1.3316564559936523
  },
  "local_rotation": 0,
  "local_scale": {
    "X": 1,
    "Y": 1
  },
  "next_sibling": "54286252720129:1723590232444558000",
  "parent": "54040865964219:1723590134132524500",
  "spawn_as_networked_entity": true
},
{
  "cid": 1,
  "aoid": "54126695379372:1723590168519319700",
  "component_type": "Internal_Component",
  "internal_component_type": "Spine_Animator",
  "data": {
    "skeleton_data_asset": "environment/portal/cosmicPortal.spine",
    "ordered_skins": [

    ],
    "depth_offset": 1.6399999856948853,
    "skeleton_scale": {
      "X": 2.4000000953674316,
      "Y": 2.4000000953674316
    },
    "mask_in_shadow": false
  }
},
{
  "cid": 3,
  "aoid": "54126695549041:1723590168519387300",
  "component_type": "Internal_Component",
  "internal_component_type": "Interactable",
  "data": {
    "prompt_offset": {
      "X": 0,
      "Y": 1
    },
    "text": "Teleport to the Hub",
    "hold_text": "",
    "radius": 2,
    "required_hold_time": 0.6000000238418579
  }
},
{
  "cid": 2,
  "aoid": "54126695616870:1723590168519414400",
  "component_type": "Mono_Component",
  "mono_component_type": "ZoneTeleporter",
  "data": {
    "ChangeStatusTo": "Safe"
  }
}
