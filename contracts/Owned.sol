// SPDX-License-Identifier: MIT
pragma solidity ^0.7.0;

import './Initializable.sol';
/**
 * @title The Owned contract
 * @notice A contract with helpers for basic contract ownership.
 */
contract Owned is Initializable {

  address payable public owner;
  address private pendingOwner;

  event OwnershipTransferRequested(
    address indexed from,
    address indexed to
  );
  event OwnershipTransferred(
    address indexed from,
    address indexed to
  );

  // constructor() {
  //   owner = msg.sender;
  // }

  /**
  * @dev Initializes the contract setting the deployer as the initial owner.
  */
  function __Owned_init() internal initializer {
      __Owned_init_unchained();
  }

  function __Owned_init_unchained() internal initializer {
      _setOwner(msg.sender);
  }

  function _setOwner(address newOwner) private {
      address oldOwner = owner;
      owner = payable(newOwner);
      emit OwnershipTransferred(oldOwner, newOwner);
  }

  /**
   * @dev Allows an owner to begin transferring ownership to a new address,
   * pending.
   */
  function transferOwnership(address _to)
    external
    onlyOwner()
  {
    pendingOwner = _to;

    emit OwnershipTransferRequested(owner, _to);
  }

  /**
   * @dev Allows an ownership transfer to be completed by the recipient.
   */
  function acceptOwnership()
    external
  {
    require(msg.sender == pendingOwner, "Must be proposed owner");

    address oldOwner = owner;
    owner = msg.sender;
    pendingOwner = address(0);

    emit OwnershipTransferred(oldOwner, msg.sender);
  }

  /**
   * @dev Reverts if called by anyone other than the contract owner.
   */
  modifier onlyOwner() {
    require(msg.sender == owner, "Only callable by owner");
    _;
  }

}
